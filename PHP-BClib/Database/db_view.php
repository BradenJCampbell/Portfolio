<?php
	namespace BClib\Database;
	
	class db_view
	{
		public $Name;
		public $Base;
		public $Where;
		
		function __construct($gen, $output, $schema, $name)
		{
			$this->_exists = false;
			$this->gen = $gen;
			$this->output = $output;
			$this->parent = $schema;
			$this->Name = $name;
			$this->cols = [];
			$this->_base = NULL;
			$this->_joins = [];
			$this->_where = NULL;
		}
		
		function __get($name)
		{
			switch ($name)
			{
				case "Columns":
					return \array_keys($this->cols);
					break;
				case "FullName":
					return $this->parent->FullName . ".`$this->Name`";
					break;
				case "Schema":
					return $this->parent->Name;
					break;
				case "Exists":
					return $this->_exists;
					break;
			}
		}
		
		function Column($alias, $column = NULL)
		{
			if (!is_null($column))
			{
				$this->cols[$alias] = new db_view_column($this, $alias, $column);
			}
			if (\array_key_exists($alias, $this->cols))
			{
				return $this->cols[$alias];
			}
			return NULL;
		}
		
		function Join($type, $table, $on)
		{
			$ret = new db_view_join($this->output, $type, $table, $on);
			array_push($this->_joins, $ret);
			return $ret;
		}
		
		function LeftJoin($table, $on)
		{
			return $this->Join("LEFT", $table, $on);
		}
		
		function MakeSelect($cols_arr, $where = NULL)
		{
			$arr = (array) $cols_arr;
			foreach ($arr as $id => $col)
			{
				if (array_key_exists($col, $this->cols))
				{
					$arr[$id] = "`" . $col . "`";
				}
			}
			$ret = "SELECT " . join(", ", (array) $arr) . " FROM `$this->Schema`.`$this->Name`";
			if (is_null($where))
			{
				return $ret;
			}
			return $ret .  " WHERE $where";
		}

		function Make()
		{
			$this->output->Write("CREATE ALGORITHM = UNDEFINED DEFINER = `root`@`localhost` SQL SECURITY DEFINER VIEW $this->FullName AS SELECT");
			$passed = false;
			foreach ($this->cols as $alias => $col)
			{
				if ($passed)
				{
					$this->output->Write(",");
				}
				$this->output->Write("\n    $col->FullTarget AS `$alias`");
				$passed = true;
			}
			$this->output->WriteLine();
			$this->output->WriteLine("FROM");
			$this->output->Indent(4);
			if (\is_object($this->Base))
			{
				$this->output->Write($this->Base->FullName);
			}
			else
			{
				$this->output->Write($this->Base);
			}
			$passed = false;
			foreach ($this->_joins as $join)
			{
				$this->output->WriteLine();
				$join->Make();
			}
			$this->output->WriteLine();
			if (!\is_null($this->Where) && $this->Where !== "")
			{
				$this->output->Write("WHERE ");
				$this->output->WriteLine($this->Where);
			}
			
			$this->output->Unindent(4);
			$this->output->WriteDelim();
		}
		
		private $_exists;
		private $gen;
		private $output;
		private $parent;
		private $cols;
		private $_joins;
	}
	
	class db_view_column
	{
		public function __construct($parent, $alias, $target)
		{
			$this->_parent = $parent;
			$this->_alias = $alias;
			$this->_targ_col = $target;
		}
		
		public function __get($name)
		{
			switch ($name)
			{
				case "FullName":
					return $this->_parent->FullName . ".`$this->Name`";
					break;
				case "Name":
					return $this->_alias;
					break;
				case "FullTarget":
					if (is_object($this->_targ_col))
					{
						return $this->_targ_col->FullName;
					}
					else
					{
						return $this->_targ_col;
					}
					break;
			}
		}
		
		private $_targ_col;
		private $_parent;
		private $_alias;		
	}
	
	class db_view_join
	{
		public $Type;
		public $Alias;
		public $Table;
		public $On;
		
		public function __construct($output, $type, $table, $on)
		{
			$this->_output = $output;
			$this->Type = $type;
			$this->Alias = NULL;
			$this->Table = $table;
			$this->On = $on;
		}
		
		public function Make()
		{
			$this->_output->Write("$this->Type JOIN ");
			if (is_object($this->Table))
			{
				$this->_output->Write($this->Table->Fullname);
			}
			else
			{
				$this->_output->Write($this->Table);
			}
			if (!is_null($this->Alias))
			{
				$this->_output->Write(" AS `$this->Alias`");
			}
			$this->_output->WriteLine();
			$this->_output->Write("    ON $this->On");
		}
		
		private $_output;
	}
?>
