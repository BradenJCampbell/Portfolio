<?php
	namespace BClib\Structures;
	
	class EdgeSet implements ArrayAccess
	{
		public $Default;
		public $Value;
		
		public function __construct()
		{
			$this->_data = [];
		}
		
		public function offsetExists(
		private $_data;
	}
	
	class edge_set_arr implements ArrayAccess
	{
		public $Default;
		
		public function __construct($default = NULL)
		{
			$this->_data = [];
			$this->Default = $default;
		}
		
		public function offsetExists($offset)
		{
			return array_key_exists($offset, $this->_data);
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_data[$offset];
			}
			return $this->Default;
		}
		
		public function offsetSet($offset, $value)
		{
			$this->_data[$offset] = $value;
		}
		
		public function offsetUnset($offset)
		{
			if ($this->offsetExists($offset))
			{
				unset($this->_data[$offset]);
			}
		}
	}
?>