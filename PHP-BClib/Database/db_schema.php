<?php
	namespace BClib\Database;
	
	require_once(__DIR__ . "/db_common.php");
	require_once(__DIR__ . "/db_table.php");
	require_once(__DIR__ . "/db_function.php");
	require_once(__DIR__ . "/db_view.php");
	
	class db_schema
	{
		private $output;
		public $Name;
		private $gen;
		private $tbls;
		private $procs;
		private $views;
		private $ordering;
		
		public function __construct($gen, $ordering, $output, $name)
		{
			$this->gen = $gen;
			$this->output = $output;
			$this->Name = $name;
			$this->tbls = [];
			$this->procs = [];
			$this->views = [];
			$this->ordering = $ordering;
			$this->ordering->Add($this);
		}
		
		public function __get($name)
		{
			switch ($name)
			{
				case "FullName":
					return "`$this->Name`";
					break;
			}
		}
		
		public function Table($name = false)
		{
			if (!$name)
			{
				$ret = array();
				foreach ($this->tbls as $in => $tbl)
				{
					if ($tbl->Exists)
					{
						array_push($ret, $in);
					}
				}
				return $ret;
			}
			if (!array_key_exists($name, $this->tbls))
			{
				$this->tbls[$name] = new db_table($this->gen, $this->output, $this, $name);
				$this->ordering->Add($this->tbls[$name]);
			}
			return $this->tbls[$name];			
		}
		
		public function Procedure($name)
		{
			if (!array_key_exists($name, $this->procs))
			{
				$this->procs[$name] = new db_function($this->gen, $this->output, $this, $name);
				$this->ordering->Add($this->procs[$name]);
			}
			return $this->procs[$name];
		}
		
		public function View($name)
		{
			if (!array_key_exists($name, $this->views))
			{
				$this->views[$name] = new db_view($this->gen, $this->output, $this, $name);
				$this->ordering->Add($this->views[$name]);
			}
			return $this->views[$name];
		}
		
		public function Validate()
		{
			$err = [];
			foreach ($this->tbls as $tbl)
			{
				if ($tbl->Validate())
				{
					$err[$tbl->Name] = $tbl->Validate();
					$tbl->Make();
				}
			}
			if ($err)
			{
				return;
			}
		}
		
		public function Make()
		{
			$err = false;
			if ($this->Validate())
			{
				foreach ($this->tbls as $tbl)
				{
					if ($tbl->Validate())
					{
						$err = true;
						$tbl->Make();
					}
				}
			}
			if ($err)
			{
				return;
			}
			$this->output->WriteLine("DROP SCHEMA IF EXISTS `$this->Name`");
			$this->output->WriteDelim();
			$this->output->WriteLine("CREATE SCHEMA `$this->Name` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */");
			$this->output->WriteDelim();
			
			foreach($this->tbls as $tbl)
			{
				//$tbl->Make();
			}
			foreach ($this->tbls as $tbl)
			{
				$tbl->MakeIndicies();
			}
			foreach ($this->procs as $proc)
			{
				//$proc->Make();
			}
		}
	}
?>
