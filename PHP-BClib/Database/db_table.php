<?php
	namespace BClib\Database;
	
	require_once(__DIR__ . "/db_common.php");
	require_once(__DIR__ . "/db_column.php");
	require_once(__DIR__ . "/db_case.php");
	
	class db_table
	{
		private $gen;
		private $output;
		public $Name;
		private $cols;
		private $_exists;
		private $parent;
		
		function __construct($gen, $output, $schema, $name)
		{
			$this->_exists = false;
			$this->gen = $gen;
			$this->output = $output;
			$this->parent = $schema;
			$this->Name = $name;
			$this->cols = [];
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
				case "Schema":
					return $this->parent->Name;
					break;
				case "Exists":
					return $this->_exists;
					break;
			}
		}
		function Column($name)
		{
			if (!array_key_exists($name, $this->cols))
			{
				$this->cols[$name] = new db_column($this->gen, $this->output, $this, $name);
				$this->_exists = true;
			}
			return $this->cols[$name];
		}
		function Cases($name)
		{
			if (!array_key_exists($name, $this->cols))
			{
				$this->cols[$name] = new db_case($this->gen, $this->output, $name);
				$this->_exists = true;
			}
			return $this->cols[$name];
		}
		function Validate()
		{
			$errs = array();
			foreach ($this->cols as $col)
			{
				if (!$col->Type)
				{
					$errs[$col->FullName] = "`$this->Schema`.`$this->Name`.`$col->Name` does not have a type";
				}
			}
			if (count($errs) > 0)
			{
				return $errs;
			}
			return false;
		}
		function Mimic($other_table)
		{
			foreach ($other_table->cols as $col)
			{
				$this->Column($col->Name)->Mimic($col);
			}
		}
		function Template($template)
		{
			if ($this->gen->Templates->Table($template)->Exists)
			{
				$this->Mimic($this->gen->Templates->Table($template));
			}
		}
		function MakeInsert($field_arr)
		{
			if (!$this->Exists)
			{
				return;
			}
			$ret = "INSERT INTO `$this->Schema`.`$this->Name`(";
			$passed = false;
			foreach ($field_arr as $field => $value)
			{
				if ($passed)
				{
					$ret .= ",";
				}
				$ret .= "`$field`";
				$passed = true;
			}
			$ret .= ") VALUES (";
			$passed = false;
			foreach ($field_arr as $field => $value)
			{
				if ($passed)
				{
					$ret .= ",";
				}
				$ret .= $value;
				$passed = true;
			}
			$ret .= ")";
			return $ret;
		}
		function MakeSelect($cols_arr, $where = NULL)
		{
			if (!$this->Exists)
			{
				return;
			}
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
		function MakeUpdate($cols_arr, $where)
		{
			if (!$this->Exists)
			{
				return;
			}
			$sets = [];
			foreach ($cols_arr as $field => $value)
			{
				array_push($sets, "`$field` = $value");
			}
			return "UPDATE `$this->Schema`.`$this->Name` SET " . join(", ", $sets) . " WHERE $where";
		}
		function Make()
		{
			if (!$this->Exists)
			{
				return;
			}
			if ($this->Validate())
			{
				foreach ($this->Validate() as $err)
				{
					$this->output->Write($err . "\n");
				}
				return;
			}
			$this->output->Write("CREATE TABLE `$this->Schema`.`$this->Name` (\n");
			$this->output->Write("    `id` bigint NOT NULL AUTO_INCREMENT,\n");
			foreach ($this->cols as $col)
			{
				$col->Make(4);
				$this->output->WriteLine(",");
			}
			$this->output->Write("    PRIMARY KEY (`id`)\n");
			$this->output->Write(") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci\n");
			$this->output->WriteDelim();
		}
		function MakeIndicies()
		{
			if (!$this->Exists)
			{
				return;
			}
			foreach ($this->cols as $col)
			{
				if ($col->Index || $col->Relation)
				{
					$this->output->WriteLine("ALTER TABLE `$this->Schema`.`$this->Name` ADD INDEX `" . $col->Name . "_idx` (`$col->Name` ASC)");
					$this->output->WriteDelim();
				}
				if ($col->Relation and $col->Relation !== $this->Name)
				{
					$this->output->WriteLine("ALTER TABLE `$this->Schema`.`$this->Name` ADD CONSTRAINT `fk_" . $this->Name . "_$col->Name` FOREIGN KEY (`$col->Name`) REFERENCES `$this->Schema`.`$col->Relation`(`id`)");
					$this->output->WriteDelim();
				}
			}
		}
	}
?>
