<?php
	namespace BClib\Database;
	
	require_once(__DIR__ . "/../Structures/GetterSetter.php");
	require_once(__DIR__ . "/../Regex.php");
	require_once(__DIR__ . "/../Error.php");
	require_once(__DIR__ . "/../String.php");
	
	class db_query
	{
		public function __construct($connection, $query)
		{
			$this->con = $connection;
			$this->query = $query;
			$this->_prep = false;
			$this->params = new db_query_params($query);
		}
		
		public function __get($name)
		{
			switch ($name)
			{
				case "Params":
				case "Parameters":
					return $this->params;
					break;
				case "Query":
					return $this->query;
					break;
			}
		}
		
		public function Exec($args_hash = [])
		{
			$missing = $this->Params->Missing($args_hash);
			if ($missing)
			{
				\BClib\Error::Output("query execution for $this->query missing parameters [" . \join(", ", $missing) . "]");
				return false;
			}
			$params = $this->Params->Resolve($args_hash);
			if ($params === false)
			{
				\BClib\Error::Output("Query $this->Query failed due to too few parameters");
				return false;
			}
			$err = [];
			try 
			{
				if ($this->prep()->execute($params))
				{
					//  query succeeded
					$ret = $this->prep()->fetchAll(\PDO::FETCH_ASSOC);
					$this->prep()->closeCursor();
					return $ret;
				}
				else
				{
					\array_push($err, \BClib\String::ImplodeArray($this->prep()->errorInfo()));
				}
			} 
			catch (\Exception $ex) 
			{
				\array_push($err, $ex);
			}
			//  query failed
			\array_unshift($err, "query execution for $this->query failed - using [" . \BClib\String::ImplodeHash($params) . "]");
			\BClib\Error::Output(\join("<br>", $err));
			return false;	
		}
		
		public function __toString()
		{
			return $this->query;
		}
		
		private function prep()
		{
			if (!$this->_prep)
			{
				//  need to prep
				$this->_prep = $this->con->prepare($this->query);
			}
			return $this->_prep;
		}
		
		private $query;
		private $_prep;
		private $params;
		private $con;
	}
	
	class db_query_params extends \BClib\Structures\GetterSetter
	{
		public function __construct($query)
		{
			$this->params = false;
			$matches_ord = \BClib\regex_all("/:[A-Za-z_]+/", $query);
			if (\count($matches_ord) > 0)
			{
				$this->params = [];
				$keys = \array_keys($matches_ord);
				\ksort($keys);
				foreach($keys as $ind)
				{
					$this->params[$ind] = $matches_ord[$ind];
				}
			}
			else
			{
				$matches_ord = \BClib\regex_all("/\?/", $query);
				$this->params = \count($matches_ord);
			}
		}
		
		public function get_IsAssoc()
		{
			return \is_array($this->params);
		}
		
		public function get_Count()
		{
			if ($this->IsAssoc)
			{
				return \count($this->params);
			}
			return $this->params;
		}
		
		public function get_List()
		{
			if ($this->IsAssoc)
			{
				return $this->params;
			}
			return false;
		}
		
		public function Missing($params_arr)
		{
			if ($this->IsAssoc)
			{
				$ret = [];
				foreach ($this->List as $param)
				{
					if (!\array_key_exists($param, $params_arr))
					{
						\array_push($ret, $param);
					}
				}
				if (count($ret) > 0)
				{
					return $ret;
				}
			}
			elseif ($this->Count > \count($params_arr))
			{
				return $this->Count - \count($params_arr);
			}
			return false;
		}
		
		public function Resolve($params_arr)
		{
			if ($this->IsAssoc)
			{
				$ret = [];
				foreach ($this->List as $param)
				{
					if (\array_key_exists($param, $params_arr))
					{
						$ret[$param] = $params_arr[$param];
					}
					else
					{
						return false;
					}
				}
				return $ret;
			}
			elseif (\count($params_arr) >= $this->Count)
			{
				$ret = [];
				for ($i = 0; $i < \count($this->params); $i++)
				{
					\array_push($ret, \array_shift($params_arr));
				}
				return $ret;
			}
			return false;
		}
		
		private $params;
	}	
?>