<?php
	namespace BClib\Structures;
	
	class NestedHash implements \ArrayAccess
	{
		public $Default;

		public function __construct($parent = NULL)
		{
			$this->_exists = [];
			$this->_data = [];
			$this->_parent = $parent;
			$this->_value_set = false;
		}
		
		public function __get($name)
		{
			switch($name)
			{
				case "Value":
					if ($this->value_set)
					{
						return $this->_value;
					}
					if (is_null($this->_parent))
					{
						return NULL;
					}
					return $this->_parent->Default;
					break;
				case "Exists":
					return $this->_value_set || count($this->_exists) > 0;
					break;
				case "Keys":
					return array_keys($this->_exists);
					break;
			}
		}
		
		public function __set($name, $value)
		{
			switch($name)
			{
				case "Value":
					$this->_value = $value;
					$this->register_with_parent();
					$this->_value_set = true;
					break;
			}
		}
		
		public function Has($offset)
		{
			return array_key_exists($osset, $this->_exists);
		}
		
		public function offsetExists($offset)
		{
			return array_key_exists($offset, $this->_data);
		}
		
		public function offsetGet($offset)
		{
			if (!$this->offsetExists($offset))
			{
				$this->_data[$offset] = new NestedHash($this);				
			}
			return $this->_data[$offset];
		}
		
		public function offsetSet($offset, $value)
		{
			if (!$this->offsetExists($offset))
			{
				$this->_data[$offset] = new NestedHash($this);
				
			}
			$this->_data[$offset]->Value = $value;
		}
		
		public function offsetUnset($offset)
		{
			if ($this->offsetExists($offset))
			{
				unset($this->_data[$offset]);
			}
		}
		
		private function register_with_parent()
		{
			if (!is_null($this->_parent))
			{
				foreach ($this->_parent->_data as $name => $item)
				{
					if ($this === $item)
					{
						$this->_parent->_exists[$name] = true;
					}
				}
			}
		}
		
		private $_exists;
		private $_data;
		private $_parent;
		private $_value;
		private $_value_set;
	}
?>