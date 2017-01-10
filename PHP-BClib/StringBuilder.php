<?php
	namespace BClib;
	
	require_once(__DIR__ . "/Writer.php");
	
	class StringBuilder extends Writer
	{
		public function __construct()
		{
			$this->filename = \tempnam(\sys_get_temp_dir(), "sb");
			parent::__construct(\fopen($this->filename, "w"));
		}
		
		public function __toString()
		{
			return \file_get_contents($this->filename);
		}
		
		public function __destroy()
		{
			parent::__destroy();
			\unlink($this->filename);
		}
		
		private $filename;
	}
?>
