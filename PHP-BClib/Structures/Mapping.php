<?php
	namespace BClib\Structures;
	
	class Mapping
	{
		public function __construct()
		{
			$this->_data = [];
		}
		
		public function Add($parent, $child)
		{
			if (!array_key_exists($parent, $this->_data))
			{
				$this->_data[$parent] = [];
			}
			$this->_data[$parent][$child] = true;
		}
		
		public function Get($parent)
		{
			if (array_key_exists($parent, $this->_data))
			{
				return array_keys($this->_data[$parent]);
			}
			return [$parent];
		}
	}
?>
