<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	
	class Input extends \BClib\Html\Tags\_base\tag_base
	{
		public function __construct()
		{
			$this->_from_template("Input");
			$this->_forms = [];
		}
		
		public function Form($name)
		{
			$this->_forms[$name] = true;
			self::_knowledge()->Form($name);
		}
		
		public function BeforeMake()
		{
			parent::BeforeMake();
			if (count($this->_forms) > 0)
			{
				$this->Attributes["form"] = "'" . \join(" ", \array_keys($this->_forms)) . "'";
			}
		}
		
		private $_forms;	
	}

	class Submit extends \BClib\Html\Tags\_base\tag_base
	{
		public function __construct()
		{
			$this->_from_template("Input");
			$this->_forms = [];
		}
		
		public function set__Caption($value)
		{
			$this->Attributes["value"] = "'$value'";
		}
		
		public function get__Caption()
		{
			return $this->Attributes["value"];
		}
		
		public function Form($name)
		{
			$this->_forms[$name] = true;
			self::_knowledge()->Form($name);
		}
		
		public function BeforeMake()
		{
			parent::BeforeMake();
			$this->Attributes["type"] = "'submit'";
			if (count($this->_forms) > 0)
			{
				$this->Attributes["form"] = "'" . \join(" ", \array_keys($this->_forms)) . "'";
			}
		}
		
		private $_forms;	
	}
?>	
