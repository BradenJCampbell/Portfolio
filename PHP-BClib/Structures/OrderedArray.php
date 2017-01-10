<?php
	namespace BClib;
	
	class OrderedArray implements \ArrayAccess
	{
		public function __construct()
		{
			$this->_data = [];
			$this->_next_index = 1;
			foreach (func_get_args() as $value)
			{
				$this->Push($value);
			}
		}
		
		public function __get($name)
		{
			switch($name)
			{
				case "Count":
					return count($this->_data);
					break;
				case "Keys":
					return array_keys($this->_data);
					break;
			}
		}
		
		public function Shift()
		{
			return array_shift($this->_data);
		}
		
		public function Unshift()
		{
			foreach (array_reverse(func_get_args()) as $value)
			{
				array_unshift($this->_data, $value);
			}
		}
		
		public function Push()
		{
			foreach (func_get_args() as $value)
			{
				array_push($this->_data, $value);
			}
		}
		
		public function Pop()
		{
			return array_pop($this->_data);
		}
		
		
		public function offsetExists($offset)
		{
			return array_key_exists($offset - 1, $this->_data);
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_data[$offset - 1];
			}
		}
		
		public function offsetSet($offset, $value)
		{
			if ($offset >= 1)
			{
				for ($i = $this->Count + 1; $i < $offset; $i++)
				{
					if (!$this->offsetExists($i))
					{
						$this->_data[$i - 1] = false;
					}
				}
				$this->_data[$offset - 1] = $value;
			}
		}
		
		public function offsetUnset($offset)
		{
			
		}
		
		private $_data;
		private $_next_index;
	}
?>
