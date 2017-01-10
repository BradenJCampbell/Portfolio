<?php
	namespace BClib\Structures;
	
	class TemplatedArray implements \ArrayAccess
	{
		public function __construct()
		{
			$this->_data = [];
			$this->_add_attribs(\func_get_args());
		}
		
		public function offsetExists($offset)
		{
			return array_key_exists($offset, $this->_data);
		}
		
		public function offsetSet($offset, $value)
		{
			if ($this->offsetExists($offset))
			{
				$this->_data[$offset] = $value;
			}
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_data[$offset];
			}
		}
		
		public function offsetUnset($offset)
		{
			
		}
		
		public function __set($field, $value)
		{
			return $this->offsetSet($field, $value);
		}
		
		public function __get($field)
		{
			if ($field === "Attributes")
			{
				return array_keys($this->_data);
			}
			return $this->offsetGet($field);
		}
		
		protected function _add_attribs($arr)
		{
			foreach ($arr as $at)
			{
				if (is_array($at))
				{
					$this->_add_attribs($at);
				}
				elseif ($at !== "Attributes" && $at !== false)
				{
					$this->_data[$at] = NULL;
				}
			}
		}
		
		private $_data;
	}
?>
