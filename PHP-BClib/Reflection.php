<?php
	namespace BClib;
	
	require_once(__DIR__ . "/Error.php");
	require_once(__DIR__ . "/Structures/GetterSetter.php");
	
	class Reflection extends \BClib\Structures\GetterSetter implements \ArrayAccess
	{
		public function __construct($class = false)
		{
			$this->_children = [];
			if (is_string($class))
			{
				$p = \explode("\\", $class);
				$this->_class = \array_pop($p);
				if (count($p) > 0)
				{
					$this->_namespace = \join("\\", $p);
				}
			}
			foreach (\get_declared_classes() as $declared_class)
			{
				if (\get_parent_class($declared_class) === $class)
				{
					$this->_children[$declared_class] = new Reflection($declared_class);
				}
			}
		}
		
		public function get_Class()
		{
			if (is_null($this->_class))
			{
				return "";
			}
			return $this->_class;
		}
		
		public function get_Namespace()
		{
			if (is_null($this->_namespace))
			{
				return "";
			}
			return $this->_namespace;
		}
		
		public function get_Children()
		{
			return \array_keys($this->_children);
		}

		public function get_FullClass()
		{
			if (is_null($this->_namespace))
			{
				return $this->Class;
			}
			return $this->_namespace . "\\" . $this->_class;
		}
		
		public function offsetExists($offset)
		{
			return \array_key_exists($offset, $this->_children);
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_children[$offset];
			}
			\BClib\Error::Output("Reflection - class '$offset' not found");
		}
		
		public function offsetSet($offset, $value)
		{
			\BClib\Error::Output("Reflection - data is read only");
		}
		
		public function offsetUnset($offset)
		{
			\BClib\Error::Output("Reflection - data is read only");
		}
				
		public function HasDescendant($class)
		{
			if ($this->Class === $class)
			{
				return true;
			}
			foreach ($this->_children as $child)
			{
				if ($child->HasDescendant($class))
				{
					return true;
				}
			}
			return false;
		}
		
		public function __toString()
		{
			$ret = [];
			return \join("\n", $this->_str("", $ret));
		}
		
		private function _str($so_far, &$ret)
		{
			if (count($this->_children) > 0)
			{
				foreach ($this->_children as $index => $child)
				{
					$child->_str($so_far . "[$index]", $ret);
				}
			}
			else
			{
				array_push($ret, $so_far);
			}
			return $ret;
		}
		
		private $_class;
		private $_namespace;
		private $_children;		
	}
?>
