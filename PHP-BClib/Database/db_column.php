<?php
	namespace BClib\Database;
	
	class db_column
	{
		private $gen;
		private $output;
		public $Name;
		private $_exists;
		private $data;
		private $parent;
		function __construct($gen, $output, $table, $name)
		{
			$this->gen = $gen;
			$this->output = $output;
			$this->parent = $table;
			$this->_exists = false;
			$this->Name = $name;
			$this->data = 
			[
				"Type" => false,
				"Default" => false,
				"Nullable" => false,
				"Collation" => false,
				"Index" => false,
				"Relation" => false,
				"Unique" => false
			];
		}
		function __get($name)
		{
			switch ($name)
			{
				case "FullName":
					return $this->parent->FullName . ".`$this->Name`";
					break;
				case "Exists":
					return $this->_exists;
					break;
			}
			if (array_key_exists($name, $this->data))
			{
				return $this->data[$name];
			}
		}
		function __set($name, $value)
		{
			if (array_key_exists($name, $this->data))
			{
				$this->data[$name] = $value;
				$this->_exists = true;
			}
		}
		function Relation($table)
		{
			$this->Template("id");
			$this->Relation = $table;
		}
		function Mimic($other_column)
		{
			if (!$other_column->Exists)
			{
				return;
			}
			$this->Type = $other_column->Type;
			$this->Default = $other_column->Default;
			$this->Nullable = $other_column->Nullable;
			$this->Collation = $other_column->Collation;
			$this->Index = $other_column->Index;
			$this->Relation = $other_column->Relation;
			$this->Unique = $other_column->Unique;
		}
		function Template($template)
		{
			if ($this->gen->Templates->Column($template)->Exists)
			{
				$this->Mimic($this->gen->Templates->Column($template));
			}
		}
		function Make($indent = 0)
		{
			if (!$this->Exists)
			{
				return;
			}
			for ($i =0; $i < $indent; $i++)
			{
				$this->output->Write(" ");
			}
			$this->output->Write("`$this->Name` $this->Type");
			if ($this->Collation)
			{
				$this->output->Write(" COLLATE $this->Collation");
			}
			if (!$this->Nullable)
			{
				$this->output->Write(" NOT NULL");
			}
			if ($this->Unique)
			{
				$this->output->Write(" UNIQUE");
			}
			if ($this->Default)
			{
				$this->output->Write(" DEFAULT $this->Default");
			}
		}
	}
?>
