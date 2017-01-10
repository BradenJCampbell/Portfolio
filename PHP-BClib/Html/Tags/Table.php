<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	
	class Table extends \BClib\Html\Tags\_base\tag_base implements \ArrayAccess
	{
		public function __construct()
		{
			$this->_from_template("Table");
		}
		
		public function offsetExists($offset)
		{
			return $this->_get_contents("Tr")->offsetExists($offset);
		}
		
		public function offsetGet($offset)
		{
			return $this->_get_contents("Tr")->offsetGet($offset);
		}
		
		public function offsetSet($offset, $value)
		{
			
		}
		
		public function offsetUnset($offset)
		{
			return $this->_get_contents("Tr")->offsetUnset($offset);
		}
	}
	
	class Tr extends \BClib\Html\Tags\_base\tag_base implements \ArrayAccess
	{
		public function __construct()
		{
			$this->_from_template("Tr");
		}
		
		public function offsetExists($offset)
		{
			return array_key_exists($offset, $this->_index);
		}
		
		public function offsetGet($offset)
		{
			return $this->_get_contents("Td")->offsetGet($offset);
		}
		
		public function offsetSet($offset, $value)
		{
			
		}
		
		public function offsetUnset($offset)
		{
			return $this->_get_contents("Td")->offsetUnset($offset);
		}
	}
	
	class Td extends \BClib\Html\Tags\_base\tag_base
	{
		public function __construct()
		{
			$this->_from_template("Td");
		}
		
		public function set__IsHeader($value)
		{
			if ($value === true)
			{
				$this->_name = "Th";
			}
			if ($value === false)
			{
				$this->_name = "Td";
			}
		}
		
		public function get__IsHeader()
		{
			return $this->_name === "Th";
		}
	}
?>
			