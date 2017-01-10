<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	require_once(__DIR__ . "/ScrollPane.php");
	require_once(__DIR__ . "/../Style.php");
	
	class Tabs extends \BClib\Html\Tags\_base\tag_base implements \ArrayAccess
	{
		public $UpArrow;
		public $DownArrow;
		public $RefreshRate;
		
		public function __construct()
		{
			self::_knowledge()->Head->Script("jquery-tabs-base")->Attributes["src"] = "'https://code.jquery.com/jquery-1.12.4.js'";
			self::_knowledge()->Head->Script("jquery-tabs-ui")->Attributes["src"] = "'https://code.jquery.com/ui/1.12.1/jquery-ui.js'";
			$this->_from_template("Div");
			$this->_cells = [];
			$this->_tab_align = "left";
			$this->_tab_style = new \BClib\Html\Style();
			$this->_tab_style["width"] = "12em";
			$this->_tab_style["text-decoration"] = "none";
			$this->_tab_style["color"] = "#000000";
			$this->_content_style = new \BClib\Html\Style();
			$this->_content_style["height"] = "100%";
			$this->_arrow_style = new \BClib\Html\Style();
			$this->_arrow_style["height"] = "20px";
			$this->_arrow_style["background-size"] = "contain";
			$this->_arrow_style["background-repeat"] = "no-repeat";
			$this->_arrow_style["background-position"] = "right";
			$this->Default;
			$this->RefreshRate = 100;
		}
		
		public function __call($name, $args)
		{
			
		}
		
		public function set__TabAlign($value)
		{
			switch($value)
			{
				case "left":
				case "right":
					$this->_tab_align = $value;
					break;
			}
		}
		
		public function get__Default()
		{
			return $this->_get_contents("Div")["default"];
		}

		public function get__TabStyle()
		{
			return $this->_tab_style;
		}

		public function get__ContentStyle()
		{
			return $this->_content_style;
		}
		public function get__TabAlign()
		{
			return $this->_tab_align;
		}
		
		public function get__ArrowStyle()
		{
			return $this->_arrow_style;
		}
		
		public function offsetExists($offset)
		{
			return \array_key_exists($offset, $this->_cells);
		}
		
		public function offsetSet($offset, $value)
		{
			
		}
		
		public function offsetGet($offset)
		{
			if (!$this->offsetExists($offset))
			{
				$next_index = \count($this->_cells) + 1;
				$new_head = $this->_get_contents("Ul")["bar"]->_get_contents("Li")[$next_index]->A(1);
				$new_body = $this->_get_contents("ScrollPane")["div" . $next_index];
				$this->_cells[$offset] = new tabs_cell($new_head, $new_body);
			}
			return $this->_cells[$offset];
		}
		
		public function offsetUnset($offset)
		{
			
		}
		
		public function BeforeMake()
		{
			$css_class_name = "ui-" . $this->Id;
			switch ($this->_tab_align)
			{
				case "left":
					$panel_float = "right";
					$tab_float = "left";
					break;
				case "right":
					$panel_float = "left";
					$tab_float = "right";
					break;
			}
			self::_knowledge()->Head->Style("jquery-ui")->Raw(".$css_class_name .ui-tabs-nav { float: $tab_float; width: " . $this->_tab_style['width'] . "; }");
			self::_knowledge()->Head->Style("jquery-ui")->Raw(".$css_class_name .ui-tabs-nav li { clear: left; width: 100%; border-bottom-width: 1px !important; border-right-width: 0 !important; list-style-type: none; }");
			self::_knowledge()->Head->Style("jquery-ui")->Raw(".$css_class_name .ui-tabs-nav li a { display:block; }");
			self::_knowledge()->Head->Style("jquery-ui")->Raw(".$css_class_name .ui-tabs-nav li.ui-tabs-active { padding-bottom: 0; padding-right: .1em; border-right-width: 1px; }");
			self::_knowledge()->Head->Style("jquery-ui")->Raw(".$css_class_name .ui-tabs-panel { float: $panel_float; width: " . $this->_content_style['width'] . "; }");
			foreach ($this->_cells as $id => $cell)
			{
				$cell->Id = $this->Id . "_" . $id;
				$cell->Tab->Style->Absorb($this->_tab_style);
				$cell->Box->Style->Absorb($this->_content_style);
				$cell->Box->ArrowStyle->Absorb($this->_arrow_style);
				$cell->Box->UpArrow = $this->UpArrow;
				$cell->Box->DownArrow = $this->DownArrow;
				if (\is_null($this->Style["height"]))
				{
					$cell->Box->Style["height"] = "99%";
				}
				else
				{
					$cell->Box->Style["height"] = $this->Style["height"];
				}
				$cell->RefreshRate = $this->RefreshRate;
			}
			self::_knowledge()->Head->Script("jquery-tabs")->Raw("$( function() {");
			self::_knowledge()->Head->Script("jquery-tabs")->Raw("    $('#" . $this->Id . "').tabs().addClass('$css_class_name ui-helper-clearfix');");
			self::_knowledge()->Head->Script("jquery-tabs")->Raw("    $('#" . $this->Id . " li').removeClass('ui-corner-top').addClass('ui-corner-left');");
			self::_knowledge()->Head->Script("jquery-tabs")->Raw("} );");
			parent::BeforeMake();
		}
		
		private $_default;
		private $_cells;
		private $_tab_align;
		private $_tab_style;
		private $_content_style;
		private $_arrow_style;
	}
	
	class tabs_cell
	{
		public function __construct($new_head, $new_body)
		{
			$this->_head = $new_head;
			$this->_body = $new_body;
		}
		
		public function __get($name)
		{
			switch ($name)
			{
				case "Head":
				case "Tab":
					return $this->_head;
					break;
				case "Box":
					return $this->_body;
					break;
				case "Body":
				case "Content":
					return $this->_body->Content;
					break;
			}
		}

		public function __set($name, $value)
		{
			switch($name)
			{
				case "Id":
					$this->_body->Id = $value;
					$this->_head->Attributes["href"] = "#" . $this->_body->Id;
					break;
			}
		}

		private $_head;
		private $_body;
	}
?>
