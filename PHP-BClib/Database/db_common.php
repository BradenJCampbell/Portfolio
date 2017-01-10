<?php
	namespace BClib\Database;
	
	class db_ordering
	{
		public function __construct()
		{
			$this->_ordering = [];
			$this->_made = [];
		}
		
		public function Add($obj)
		{
			$ret = \count($this->_ordering);
			\array_push($this->_ordering, $obj);
			return $ret;
		}
		
		public function Make($obj = NULL)
		{
			if (is_null($obj))
			{
				foreach (\array_keys($this->_ordering) as $ind)
				{
					$this->_made[$ind] = false;
				}
				foreach (\array_keys($this->_ordering) as $ind)
				{
					$this->Make($ind);
				}
			}
			else
			{
				if ($this->_made[$obj] === false)
				{
					$this->_ordering[$obj]->Make();
					$this->_made[$obj] = true;
				}
			}
		}
	}
		
	class db_templates
	{
		private $gen;
		private $schemas;
		private $tables;
		private $columns;
		
		public function __construct($gen)
		{
			$this->gen = $gen;
			$this->schemas = [];
			$this->tables = [];
			$this->columns = [];
		}
		
		public function Schema($name)
		{
			if (!array_key_exists($name, $this->schemas))
			{
				$this->schemas[$name] = new db_schema($this->gen, null, $name);
			}
			return $this->schemas[$name];
		}
		
		public function Table($name)
		{
			if (!array_key_exists($name, $this->tables))
			{
				$this->tables[$name] = new db_table($this->gen, null, $name);
			}
			return $this->tables[$name];
		}
		
		public function Column($name)
		{
			if (!array_key_exists($name, $this->columns))
			{
				$this->columns[$name] = new db_column($this->gen, null, null, $name);
			}
			return $this->columns[$name];
		}
	}
?>
