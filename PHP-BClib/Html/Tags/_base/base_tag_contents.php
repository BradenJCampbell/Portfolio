<?php
	namespace BClib\Html\Tags\_base;
	
	require_once(__DIR__ . "/base_tag.php");
	
	class base_tag_contents implements \ArrayAccess
	{
		public function __construct($type, &$index)
		{
			$this->_proto = tag_template::Get($type);
			if (!$this->_proto)
			{
				$this->_proto = $type;
			}
			$this->_contents = [];
			$this->_index = &$index;
		}
		
		public function offsetExists($name)
		{
			return array_key_exists($name, $this->_contents);
		}
		
		public function offsetSet($name, $value)
		{
			$this->_set_item($name, $value);
		}
		
		public function offsetGet($name)
		{
			$this->_set_item($name, "");
			if ($this->offsetExists($name))
			{
				return $this->_contents[$name];
			}
		}
		
		public function offsetUnset($name)
		{
			if ($this->offsetExists($name))
			{
				unset($this->_index[$name]);
				unset($this->_contents[$name]);
			}
		}
		
		private function _set_item($name, $value)
		{
			if (!$this->offsetExists($name) && !array_key_exists($name, $this->_index))
			{
				if (is_object($this->_proto))
				{
					$this->_index[$name] = $this;
					$this->_contents[$name] = $this->_proto->create();
				}
				else
				{
					$this->_index[$name] = $this;
					$this->_contents[$name] = $value;
				}
			}
		}
		
		private $_proto;
		private $_index;		
		private $_contents;
	}
?>
