<?php
	namespace BClib\Html;
	
	require_once(__DIR__ . "/base.php");
			class menubar_bar extends html_base
	{
		public function __construct($component_name)
		{
			$this->_comp_name = $component_name;
			$this->_comp = self::_component($component_name);
			self::_knowledge()->Style([$this->_comp_name, "element"], "." . $this->_comp_name . "_element");
			self::_knowledge()->Style([$this->_comp_name, "element", "link"], 
				"." . $this->_comp_name . "_element :link, ." . $this->_comp_name . "_element :visited, ." . $this->_comp_name . "_element :active");
			self::_knowledge()->Style([$this->_comp_name, "element", "hover"], "." . $this->_comp_name . "_element :hover");
			$this->_tbl = $this->_comp->Table(self::_id($this->_comp_name, "Table"));
			$this->_tbl->Id = self::_id($this->_comp_name, "Table");
			$this->_ender = $this->_comp->Div("Ender");
			$this->_cells = [];
		}

		public function get__Ender()
		{
			$this->_ender->Id = \BClib\String::RemoveQuotes($this->_comp->Id) . "_Ender";
			return $this->_ender;
		}
		
		public function Style($name = "background")
		{
			switch ($name)
			{
				case "background":
					return $this->_comp->Style;
					break;
				case "element":
				case "element_link":
				case "element_hover":
					return self::_knowledge()->Style([$this->_comp_name, $name]);
					break;
				case "border":
					return $this->_tbl->Style;
					break;
				case "Ender":
					return $this->Ender->Style;
					break;
			}
			if (\array_key_exists($name, $this->_cells))
			{
				return $this->_cells[$name]->Style;
			}
		}
		
		public function Cell($index = NULL)
		{
			if (\is_null($index))
			{
				return \array_keys($this->_cells);
			}
			if (!\array_key_exists($index, $this->_cells))
			{
				$cell = $this->_tbl[1][$index];
				$cell->Attributes["class"] = "'" . $this->_comp_name . "_element'";
				$cell->Id = self::_id($this->_comp_name, $index);
				$this->_cells[$index] = new menubar_cell($this->_comp_name, $cell);
				$this->_cells[$index]->Id = self::_id($this->_comp_name, $index);
			}
			return $this->_cells[$index];
		}
		
		private $_comp_name;
		private $_comp;
		private $_tbl;
		private $_ender;
		private $_cells;
	}
	
	class menubar_cell extends html_base implements \ArrayAccess
	{
		public function __construct($name, $block)
		{
			$this->_comp_name = $name;
			$this->_block = $block;
			$this->_on_open("MB_Close()");
		}
		
		public function set__Image($value)
		{
			$this->_block->A("caption")->Img("image")->Attributes["src"] = $value;
		}
		
		public function set__Caption($value)
		{
			$this->_block->A("caption")->Text[0] = $value;
		}
		
		public function set__Link($value)
		{
			$this->_block->A("caption")->Attributes["href"] = $value;
		}
				public function set__Id($value)
		{
			if (is_null($this->_block->Id))
			{
				$this->_block->Id = $value;
				$this->_block->Attributes["onmouseover"] = "'javascript:MB_Open_" . $value . "();'";
			}
		}
		
		public function get__Style()
		{
			return $this->_block->Style;
		}
		
		public function get__Image()
		{
			return $this->_block->A("caption")->Img("image");
		}
		public function get__Caption()
		{
			return $this->_block->A("caption")->Text[0];
		}
		public function get__Link()
		{
			return $this->_block->A("caption")->Attributes["href"];
		}
		public function get__Id()
		{
			return $this->_block->Id;
		}
		
		public function get__Raw()
		{
			return $this->_block;
		}
		
		public function offsetExists($offset)
		{
			if (is_null($this->_children))
			{
				return false;
			}
			return \array_key_exists($offset, $this->_children);
		}
		
		public function offsetGet($offset)
		{
			if ($this->offsetExists($offset))
			{
				return $this->_children[$offset]->Text[0];
			}
			return NULL;
		}
		
		public function offsetSet($offset, $value)
		{
			$children_id = $this->_block->Id . "_Children";
			$children_up = \strtoupper($children_id);
			$children_div = self::_knowledge()->Body->Div($children_id);
			if(is_null($this->_children))
			{
				self::_knowledge()->SubAdd($children_id, $children_id);
				$children_div->Id = $children_id;
				$children_div->Attributes["class"] = "'" . $this->_comp_name . "_background'";
				$children_div->Style["z-index"] = self::_component($this->_comp_name)->Style["z-index"];
				$children_div->Style["display"] = "none";
				self::_func("MB_Close", "-$children_up-.style.display = 'none'");
				$this->_on_open("var mid = -$children_up-.offsetWidth / 2");
				$this->_on_open("-$children_up-.style.position = 'fixed'");
				//  horizontal center
				$this->_on_open("-$children_up-.style.left = AbsolutePosition(" . $this->_block->JavascriptGet() . ").midX - mid");
				//$this->_on_open(\BClib\Javascript::CenterHorizontal("-$children_up-", $this->_block->JavascriptGet()));
				$this->_on_open("-$children_up-.style.top = -MENUBAR-.offsetTop + -MENUBAR-.offsetHeight");
				$this->_on_open("-$children_up-.style.display = 'block'");
				$this->_on_open("-$children_up-.style.minWidth = document.getElementById('" . $this->_block->Id . "').offsetWidth");
				$this->_children = [];
			}
			if (!$this->offsetExists($offset))
			{
				$this->_children[$offset] = $children_div->Div($offset);
				$this->_children[$offset]->Id = $this->_block->Id . "_" . $offset;
				$this->_children[$offset]->Attributes["class"] = $this->_block->Attributes["class"];
				$this->_on_open("document.getElementById('" . $this->_children[$offset]->Id . "').minWidth = document.getElementById('" . $this->_block->Id . "').offsetWidth");
			}
			$this->_children[$offset]->A(0)->Text[1] = $value;
		}
				public function offsetUnset($offset)
		{
			
		}					
				private function _on_open($line)
		{
			self::_func("MB_Open_" . $this->Id, $line);
		}
		private $_comp_name;
		private $_block;
		private $_children;
	}
?>
