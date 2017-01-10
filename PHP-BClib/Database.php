<?php
	namespace BClib\Database;
	
	require_once(__DIR__ . "/Error.php");
	require_once(__DIR__ . "/Writer.php");
	require_once(__DIR__ . "/Database/db_common.php");
	require_once(__DIR__ . "/Database/db_schema.php");
	require_once(__DIR__ . "/Database/db_table.php");
	require_once(__DIR__ . "/Database/db_column.php");
	require_once(__DIR__ . "/Database/db_query.php");
	
	class Connection extends \PDO
	{
		public function __construct($hostname, $username, $password)
		{
			$this->_hostname = $hostname;
			$this->_username = $username;
			$this->_password = $password;
			parent::__construct("mysql:host=$this->_hostname;", $this->_username, $this->_password);
		}
		
		public function __get($name)
		{
			switch ($name)
			{
				case "Hostname":
					return $this->_hostname;
					break;
				case "Username":
					return $this->_username;
					break;
				case "Password":
					return $this->_password;
					break;
			}
			return parent::__get($name);
		}
	}
	
	class Queries
	{
		public function __construct($connection)
		{
			$this->_connection = $connection;
			$this->_queries = [];
		}
		
		public function Query($alias, $query = NULL)
		{
			if (\is_null($query))
			{
				return \array_key_exists($alias, $this->_queries);
			}
			$this->_queries[$alias] = new db_query($this->_connection, $query);
			return true;
		}
		
		public function Exec($alias, $args_arr = [])
		{
			if ($this->_has($alias))
			{
				return $this->_queries[$alias]->Exec($args_arr);
			}
			\BClib\Error::Output("query '$alias' not defined");
			return false;
		}
		
		public function __call($alias, $args_arr)
		{
			return $this->Exec($alias, $args_arr);
		}
		
		public function __toString()
		{
			$ret = [];
			foreach ($this->_queries as $alias => $query)
			{
				\array_push($ret, "$alias => $query");
			}
			return \join("\n", $ret);
		}
		
		private function _has($alias)
		{
			return \array_key_exists($alias, $this->_queries);
		}
		private $_connection;
		private $_queries;
	}

	class Generate
	{
		public function __construct()
		{
			$this->output = new \BClib\Writer();
			$this->templates = new db_templates($this);
			$this->ordering = new db_ordering();
			$this->schemas = [];
			$this->cleanup = [];
		}
		public function __get($name)
		{
			if ($name === "Templates")
			{
				return $this->templates;
			}
		}
		public function Schema($name)
		{
			if (!array_key_exists($name, $this->schemas))
			{
				$this->schemas[$name] = new db_schema($this, $this->ordering, $this->output, $name);
			}
			return $this->schemas[$name];
		}
		
		public function Cleanup($str)
		{
			array_push($this->cleanup, $str);
		}
		
		public function Validate()
		{
			$ret = true;
			foreach ($this->schemas as $schema)
			{
				if ($schema->Validate())
				{
					$ret = false;
					$schema->Make();
				}
			}
			return $ret;
		}
		
		public function Make()
		{
			if (!$this->Validate())
			{
				return;
			}
			$this->output->WriteLine("DELIMITER $$");
			$this->output->WriteLine("SET @start_time = CURRENT_TIMESTAMP");
			$this->output->WriteDelim();
			
			$this->ordering->Make();
			
			foreach ($this->cleanup as $str)
			{
				$this->output->WriteLine($str);
				$this->output->WriteDelim();
			}
			$this->output->WriteLine("SELECT concat('execution complete (took ', TIMEDIFF(CURRENT_TIMESTAMP, @start_time), ')')");
		}
		
		private $output;
		private $templates;
		private $schemas;
		private $cleanup;
		private $ordering;
	}
?>
