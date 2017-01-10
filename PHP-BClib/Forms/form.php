<?php
	class Form
	{
		public $Name;
		private $inputs;
		public function __construct($name)
		{
			$this->Name = $name;
			$this->inputs = [];
		}
		public function Input($name)
		{
			if (!array_key_exists($name, $this->inputs))
			{
				$this->inputs[$name] = new FormInput($this, $name);
			}
			return $this->inputs[$name];
		}
		public function Submit($name, $caption)
		{
			$this->Input($name)->Value = $caption;
			$this->Input($name)->Type = 'submit';
		}
		public function HasData($name = false)
		{
			foreach ($this->inputs as $in)
			{
				if ($in->HasData())
				{
					return true;
				}
			}
			return false;
		}
		public function GetData()
		{
			$ret = [];
			foreach ($this->inputs as $in)
			{
				if ($in->HasData())
				{
					$ret[$in->Name] = $in->GetData();
				}
			}
			return $ret;
		}
		public function Make()
		{
			echo "<form>";
			foreach ($this->inputs as $in)
			{
				$in->Make();
			}
			echo "</form>";
		}
	}
	
	class FormInput
	{
		public $Name;
		public $Value;
		public $Type;
		private $_parent;
		public function __construct($parent, $name)
		{
			$this->_parent = $parent;
			$this->Name = $name;
			$this->Value = "";
			$this->Type = "text";
		}
		public function FullName()
		{
			return $this->_parent->Name . "_" . $this->Name;
		}
		public function HasData()
		{
			if (array_key_exists($this->FullName(), $_POST))
			{
				return true;
			}
			return array_key_exists($this->FullName(), $_GET);
		}
		public function GetData()
		{
			if (array_key_exists($this->FullName(), $_POST))
			{
				return $_POST[$this->FullName()];
			}
			if (array_key_exists($this->FullName(), $_GET))
			{
				return $_GET[$this->FullName()];
			}
		}
		public function Make()
		{
			echo "<input name='";
			echo $this->FullName();
			echo "' value='$this->Value' type='$this->Type'>";
		}
	}
?>