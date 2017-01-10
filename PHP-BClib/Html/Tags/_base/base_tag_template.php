<?php
	namespace BClib\Html\Tags\_base;
	
	require_once(__DIR__ . "/../../../Structures/NestedHash.php");
	require_once(__DIR__ . "/../../../Reflection.php");
	require_once(__DIR__ . "/base_tag.php");
	
	class tag_template extends tag_base
	{
		protected function __construct($name, $is_atomic = false, $attribs = [])
		{
			$this->name = $name;
			$this->is_atomic = $is_atomic;
			$this->attribs = [];
			$this->parenting = new tag_template_parenting_set(true);
			foreach (self::$__common as $comm)
			{
				array_push($this->attribs, $comm);
			}
			foreach ($attribs as $at)
			{
				array_push($this->attribs, $at);
			}
		}
			
		public function create()
		{
			$class = self::_reflect($this->name);
			if ($class)
			{
				return new $class();
			}
			else
			{
				return new \BClib\Html\Tags\_base\tag_base($this->name, $this->is_atomic, $this->attribs);
			}
		}
		
		public $name;
		public $is_atomic;
		public $attribs;
		public $parenting;
		
		public static function Get($name, $parent = NULL)
		{
			self::_init();
			if (array_key_exists($name, self::$__ins))
			{
				if (is_null($parent) || self::$__ins[$name]->parenting[$parent])
				{
					return self::$__ins[$name];
				}
			}
			return NULL;
		}
		
		private static function _init()
		{
			if (!self::$__common)
			{
				self::$__common = 
				[
					"class", 
					"dir", 
					"lang", 
					"style", 
					"title", 
					"onblur", 
					"onchange", 
					"onclick", 
					"ondblclick", 
					"onfocus", 
					"onkeydown", 
					"onkeypress", 
					"onkeyup", 
					"onload", 
					"onmousedown", 
					"onmousemove", 
					"onmouseout", 
					"onmouseover",
					"onmouseup",
					"onreset",
					"onselect",
					"onsubmit",
					"onunload"
				];
			}
			self::_add("Table", false, ["border"]);
			self::_add("Tr");
			self::_add("Td", false, ["colspan", "rowspan", "align", "width", "height"]);
			self::_add("Div", false, ["align"]);
			self::_add("Img", true, ["alt", "longdesc", "height", "width", "src"]);
			self::_add("Script", false, ["type", "src"]);
			self::_add("Style");
			self::_add("A", false, ["href"]);
			self::_add("P");
			self::_add("Ul");
			self::_add("Li");
			self::_add("Br", true);
			self::_add("Form", false, ["name", "action", "method"]);
			self::_add("Input", true, ["name", "value", "type", "form"]);
			self::_add("Textarea", false, ["name", "rows", "cols"]);
			self::_add("Submit", false, ["name"]);
			self::_add("Scroller");
			self::_add("Tabs");
			self::_add("ScrollPane");
		}
		private static function _reflect($tag)
		{
			if (!self::$__reflector)
			{
				self::$__reflector = new \BClib\Reflection(\get_parent_class(__CLASS__));
			}
			if (!self::$__reflect)
			{
				self::$__reflect = [];
				$prefix = \explode("\\", __CLASS__);
				\array_pop($prefix);
				\array_pop($prefix);
				$prefix = \implode("\\", $prefix);
				foreach (\get_declared_classes() as $class)
				{
					$curr_prefix = \explode("\\", $class);
					$curr_suffix = \array_pop($curr_prefix);
					$curr_prefix = \implode("\\", $curr_prefix);
					if (
						$class !== __CLASS__ &&
						\get_parent_class($class) === \get_parent_class(__CLASS__) &&
						$curr_prefix === $prefix
					)
					{
						self::$__reflect[$curr_suffix] = $class;
					}
				}
			}
			if (array_key_exists($tag, self::$__reflect))
			{
				return self::$__reflect[$tag];
			}
			return NULL;
		}
		private static function _add($name, $is_atomic = false, $attribs = [])
		{
			if (!self::$__ins)
			{
				self::$__ins = [];
			}
			if (!array_key_exists($name, self::$__ins))
			{
				self::$__ins[$name] = new tag_template($name, $is_atomic, $attribs);
			}
		}
		
		private static $__reflector;
		private static $__reflect;
		private static $__ins;
		private static $__common;
	}
	
	class tag_template_parenting_set implements \ArrayAccess
	{
		public $Default;
		public function __construct($default = false)
		{
			$this->Default = $default;
			$this->_data = [];
		}
		
		public function offsetExists($offset)
		{
			return \array_key_exists($offset, $this->_data);
		}
		
		public function offsetSet($offset, $value)
		{
			$this->_data[$offset] = $value;
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_data[$offset];
			}
			return $this->Default;
		}
		
		public function offsetUnset($offset)
		{
			if ($this->offsetExists($offset))
			{
				unset($this->_data[$offset]);
			}
		}
		
		private $_data;
	}
?>
