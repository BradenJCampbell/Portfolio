<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	
	class Script extends \BClib\Html\Tags\_base\tag_base
	{
		public function __construct()
		{
			$this->_from_template("Script");
		}
		
	}
	
	class script_func
	{
		public function __construct()
		{
			$this->_params = [];
			$this->_code = [];
		}
		
		public function Parameter($name)
		{
			$this->_params[$name] = 1;
		}
		
		public function Code($line)
		{
			array_push($this->_code, $line);
		}
		
		private $_id;
		private $_