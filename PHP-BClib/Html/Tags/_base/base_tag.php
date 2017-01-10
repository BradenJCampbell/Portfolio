<?php
	namespace BClib\Html\Tags\_base;
	
	require_once(__DIR__ . "/../../../Structures/GetterSetter.php");
	require_once(__DIR__ . "/../../../Writer.php");
	require_once(__DIR__ . "/../../../Structures/TemplatedArray.php");
	require_once(__DIR__ . "/../../../Structures/OrderedArray.php");
	require_once(__DIR__ . "/../../Style.php");
	require_once(__DIR__ . "/../../../Error.php");
	require_once(__DIR__ . "/../../../String.php");
	require_once(__DIR__ . "/base_tag_contents.php");
	require_once(__DIR__ . "/base_tag_template.php");
	require_once(__DIR__ . "/base_knowledge.php");
	
	class tag_base
	{
		const TEXT_NAME = "Text";
		
		protected function __construct($name, $is_atomic = false, $attribs = false)
		{
			$this->_setup($name, $is_atomic, $attribs);
		}
		
		protected function _setup($name, $is_atomic = false, $attribs = false)
		{
			$this->_name = $name;
			if ($attribs and is_array($attribs))
			{
				$this->_attribs = new tag_attributes($this->_name, $attribs);
				//$this->_attribs = new \BClib\Structures\TemplatedArray($attribs);
			}
			else
			{
				$this->_attribs = new tag_attributes($this->_name);
				//$this->_attribs = new \BClib\Structures\TemplatedArray();
			}
			$this->_index = [];
			$this->_contents = [self::TEXT_NAME => new base_tag_contents(self::TEXT_NAME, $this->_index)];
			$this->is_atomic = $is_atomic;
		}
		
		protected function _from_template($name)
		{
			$proto = tag_template::Get($name);
			if ($proto)
			{
				$this->_setup($proto->name, $proto->is_atomic, $proto->attribs);
			}
		}
		
		public function set__Id($value)
		{
			$this->_id = $value;
		}
		
		public function get__Id()
		{
			return $this->_id;
		}
		
		public function get__Attributes()
		{
			return $this->_attribs;
		}
		
		public function get__Style()
		{
			if (!$this->_style)
			{
				$this->_style = new \BClib\Html\Style();
			}
			return $this->_style;
		}

		final public function __set($name, $value)
		{
			if (\method_exists($this, "set__" . $name))
			{
				\call_user_func_array([$this, "set__" . $name], [$value]);
				return;
			}
			\BClib\Error::AttributeNotFound(__CLASS__ . "(" . $this->_name . ")", $name);
		}
		
		final public function __get($name)
		{
			if (\method_exists($this, "get__" . $name))
			{
				return \call_user_func_array([$this, "get__" . $name], []);
			}
			$ret = $this->_get_contents($name);
			if ($ret)
			{
				return $ret;
			}
			\BClib\Error::AttributeNotFound(__CLASS__ . "(" . $this->_name . ")", $name);
			return NULL;
		}
		
		public function __call($name, $args)
		{
			if ($name === "Raw")
			{
				$this->_get_contents(self::TEXT_NAME)[count($this->_index) + 1] = \array_shift($args);
				return;
			}
			if ($name === "P")
			{
				$this->Raw("<p>" . \array_shift($args) . "</p>");
				return;
			}
			if ($name === "Bold")
			{
				$this->Raw("<b>" . \array_shift($args) . "</b>");
				return;
			}
			if ($name === "Br")
			{
				$this->Raw("<br>");
				return;
			}
			$ret = $this->_get_contents($name);
			if ($ret)
			{
				$id = array_shift($args);
				if (count($args) > 0)
				{
					$ret[$id] = array_shift($args);
				}
				return $ret[$id];
			}
			\BClib\Error::CallNotFound(__CLASS__ . "(" . $this->_name . ")", $name);
			return NULL;
		}
		
		public function JavascriptGet()
		{
			if (is_null($this->Id))
			{
				return NULL;
			}
			$args = func_get_args();
			array_unshift($args, "document.getElementById('" . $this->Id . "')");
			return \join(".", $args);
		}
		
		protected function _get_contents($name)
		{
			if ($name === self::TEXT_NAME)
			{
				return $this->_contents[self::TEXT_NAME];
			}
			if (!array_key_exists($name, $this->_contents))
			{
				$proto = tag_template::Get($name);
				if ($proto)
				{
					$this->_contents[$name] = new base_tag_contents($name, $this->_index);
				}
			}
			if (array_key_exists($name, $this->_contents))
			{
				return $this->_contents[$name];
			}
			return NULL;
		}
		
		protected function _make_atomic(\BClib\Writer $writer)
		{
			$writer->Write("<");
			$this->_tag_header($writer);
			$writer->WriteLine("/>");
		}
		
		protected function _make_full(\BClib\Writer $writer)
		{
			$writer->Write("<");
			$this->_tag_header($writer);
			$writer->WriteLine(">");
			$writer->Indent(4);
			$this->_tag_contents($writer);
			$writer->Unindent(4);
			$writer->Write("</");
			$writer->Write(strtolower($this->_name));
			$writer->WriteLine(">");
		}
		
		private function _tag_contents($writer)
		{
			foreach ($this->_index as $ind => $type)
			{
				$cont = $type[$ind];
				if (is_object($cont))
				{
					if (method_exists($cont, "Make"))
					{
						$cont->Make($writer);
					}
					else
					{
						$writer->WriteLine($cont->__toString());
					}
				}
				else
				{
					$writer->WriteLine($cont);
				}
			}
		}
		
		private function _tag_header(\BClib\Writer $writer)
		{
			$writer->Write(\strtolower($this->_name));
			if ($this->_id)
			{
				$writer->Write(" id='");
				$writer->Write(\BClib\String::RemoveQuotes($this->_id));
				$writer->Write("'");
			}
			if ($this->_style)
			{
				$writer->Write(" style='");
				$this->_style->Make($writer);
				$writer->Write("'");
			}
			foreach ($this->_attribs->Attributes as $attrib)
			{
				if (is_null($this->_attribs[$attrib]))
				{
				}
				else
				{
					if ($attrib === "style" && $this->_style)
					{
					}
					else
					{
						$writer->Write(" ");
						$writer->Write($attrib);
						$writer->Write("=");
						$writer->Write($this->_attribs[$attrib]);
					}
				}
			}		
		}
		
		public function BeforeMake()
		{
			foreach ($this->_index as $ind => $type)
			{
				$cont = $type[$ind];
				if (is_object($cont))
				{
					if (method_exists($cont, "BeforeMake"))
					{
						$cont->BeforeMake();
					}
				}
			}
		}
		
		public function Make(\BClib\Writer $writer)
		{
			if ($this->is_atomic)
			{
				$this->_make_atomic($writer);
			}
			else
			{
				$this->_make_full($writer);
			}
		}
		
		protected static function _knowledge()
		{
			if (is_null(self::$__knowledge))
			{
				self::$__knowledge = new tag_base_knowledge();
			}
			return self::$__knowledge;
		}
		protected $_name;
		protected $_attribs;
		protected $_contents;
		protected $_index;
		protected $_style;
		protected $is_atomic;
		private $_id;
		private static $__knowledge;
	}
	
	class tag_attributes extends \BClib\Structures\TemplatedArray
	{
		public $TagName;
		
		public function __construct()
		{
			$args = \func_get_args();
			$this->TagName = \array_shift($args);
			parent::__construct($args);
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return parent::offsetGet($offset);
			}
			\BClib\Error::Output("Tag $this->TagName does not have an attribute $offset");
		}
		
		public function offsetSet($offset, $value)
		{
			if ($offset === "name")
			{
				$value = "'" . \BClib\String::RemoveQuotes($value) . "'";
			}
			if ($this->offsetExists($offset))
			{
				parent::offsetSet($offset, $value);
				return;
			}
			\BClib\Error::Output("Tag $this->TagName does not have an attribute $offset");
		}
	}
?>
