<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	require_once(__DIR__ . "/Table.php");
	require_once(__DIR__ . "/../../String.php");
	
	class Scroller extends \BClib\Html\Tags\_base\tag_base implements \ArrayAccess
	{
		public $Columns;
		public $Rows;
		public $Width;
		public $Height;
		public $Id;
		
		public function __construct()
		{
			$this->_from_template("Div");
			$this->_viewport = $this;
			$this->_wrapper = $this->_viewport->_get_contents("Div")["wrapper"];
			$this->_scroller = $this->_wrapper->_get_contents("Div")["scroller"];
			$this->_cells = [];
			$this->_slide_style = new \BClib\Html\Style();
			$this->_animation = new scroller_animation();
			$this->_javascript = new scroller_javascript($this);
			if (is_null(self::$_js_included))
			{
				self::_knowledge()->Head->Script("Scrollers_load")->Attributes["src"] = "'iscroll-master/build/iscroll.js'";
				//$this->_script("document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);");
				self::$_js_included = true;
			}
		}
		
		public function __call($name, $args)
		{
			
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
				$this->_cells[$offset] = $this->_scroller->_get_contents("Div")[$offset];
			}
			return $this->_cells[$offset];
		}
		
		public function offsetUnset($offset)
		{
			
		}
		
		public function get__SlideStyle()
		{
			return $this->_slide_style;
		}
		
		public function get__Animation()
		{
			return $this->_animation;
		}
		
		public function get__Javascript()
		{
			return $this->_javascript;
		}
		
		public function Pattern()
		{
			$arg_list = \func_get_args();
			if (\count($arg_list) > 0)
			{
				$this->_pattern = [];
				foreach ($arg_list as $arg)
				{
					if (\array_key_exists($arg, $this->_cells))
					{
						array_push($this->_pattern, $arg);
					}
				}
			}
			if (is_null($this->_pattern))
			{
				return \array_keys($this->_cells);
			}
			return $this->_pattern;
		}
		
		public function BeforeMake()
		{
			//  viewport
			$this->_viewport->Style["position"] = "relative";
			$this->_viewport->Style["margin"] = "0 auto";
			$this->_viewport->Style["overflow"] = "hidden";
			$this->_viewport->Style["width"] = $this->Width;
			$this->_viewport->Style["height"] = $this->Height;
			//  wrapper
			$this->_wrapper->Id = "'" . $this->Id . "_wrapper'";
			$this->_wrapper->Style["width"] = $this->Width;
			$this->_wrapper->Style["height"] = $this->Height;
			$this->_wrapper->Style["margin"] = "0 auto";
			//  scroller
			$this->_scroller->Style["position"] = "absolute";
			$this->_scroller->Style["z-index"] = 1;
			$this->_scroller->Style["-webkit-tap-highlight-color"] = "rgba(0,0,0,0)";
			$this->_scroller->Style["transform"] = "translateZ(0)";
			$this->_scroller->Style["width"] = $this->Width * $this->Columns . "px";
			$this->_scroller->Style["height"] = $this->Height * $this->Rows . "px";
			$this->_scroller->Style["-webkit-touch-callout"] = "none";
			$this->_scroller->Style["-webkit-user-select"] = "none";
			$this->_scroller->Style["-moz-user-select"] = "none";
			$this->_scroller->Style["-ms-user-select"] = "none";
			$this->_scroller->Style["user-select"] = "none";
			$this->_scroller->Style["text-size-adjust"] = "none";
			foreach ($this->_cells as $id => $cell)
			{
				$cell->Id = "'" . $this->Id . "_" . $id . "'";
				foreach ($this->SlideStyle->Attributes as $style_att)
				{
					if(!\is_null($this->SlideStyle[$style_att]))
					{
						$cell->Style[$style_att] = $this->SlideStyle[$style_att];
					}
				}
				$cell->Style["width"] = $this->Width;
				$cell->Style["height"] = $this->Height;
				$cell->Style["float"] = "left";
			}
			$pattern_ids = [];
			foreach ($this->Pattern() as $pattern_id)
			{
				\array_push($pattern_ids, "'#" . \BClib\String::RemoveQuotes($this[$pattern_id]->Id) . "'");
			}
			$this->_script("var -VAR-;");
			$this->_script("var -CURR- = 0;");
			$this->_script("var -PATTERN- = [" . \join(", ", $pattern_ids) . "];");
			$this->_script("function -LOAD-() {");
			$this->_script("    -VAR- = new IScroll('#-WRAPPER-ID-', {");
			$this->_script("        scrollX: true,");
			$this->_script("        scrollY: true,");
			$this->_script("        momentum: false,");
			$this->_script("        snap: false,");
			$this->_script("        snapSpeed: 400,");
			$this->_script("        keyBindings: false");
			$this->_script("    });");
			$this->_script("    -VAR-.scrollToElement(-PATTERN-[0], 0);");
			$this->_script("    -NEXT-();");
			$this->_script("}");
			$this->_script("function -NEXT-() {");
			$this->_script("    " . $this->Animation->ScrollTo("-VAR-", "-PATTERN-[-CURR-]") . ";");
			$this->_script("    -CURR- = -CURR- + 1;");
			$this->_script("    if (-CURR- >= -PATTERN-.length) {");
			$this->_script("        -CURR- = 0;");
			$this->_script("    }");
			$this->_script("    setTimeout(-NEXT-, " . $this->Animation->SlideDelay . ");");
			$this->_script("}");
			self::_knowledge()->OnLoad($this->Id . "_load()");
			self::_knowledge()->OnResize("setTimeout(function() {" . $this->Id . "_scroll.refresh();}, 100)");
		}
		
		private function _script($str)
		{
			$mapping = [
				"-VAR-"        => $this->Id . "_scroll",
				"-CURR-"       => $this->Id . "_curr",
				"-PATTERN-"    => $this->Id . "_pattern",
				"-LOAD-"       => $this->Id . "_load",
				"-NEXT-"       => $this->Id . "_next",
				"-WRAPPER-ID-" => \BClib\String::RemoveQuotes($this->_wrapper->Id)
			];
			$add_str = $str;
			foreach ($mapping as $patt => $sub)
			{
				$add_str = \str_replace($patt, $sub, $add_str);
			}
			self::_knowledge()->Head->Script("Scrollers")->Raw($add_str);
		}
				
		private $_viewport;
		private $_wrapper;
		private $_scroller;
		private $_cells;
		private $_pattern;
		private $_slide_style;
		private $_animation;
		private $_javascript;
		private static $_js_included;
	}
	
	class scroller_javascript
	{
		public function __construct($scroller)
		{
			$this->_scroller = $scroller;
		}
		
		public function ScrollTo($index)
		{
			return $this->_scroller->Id . "_scroll.scrollToElement('" . $this->_scroller[$index]->Id . "');";
		}
		
		private $_scroller;
	}
	
	class scroller_animation
	{
		public $Time;
		public $SlideDelay;
		public $Type;
		public $OffsetX;
		public $OffsetY;
		
		public function __construct()
		{
			$this->Time = 1000;
			$this->SlideDelay = 2500;
			$this->Type = NULL;
			$this->OffsetX = 0;
			$this->OffsetY = 0;
		}
		
		public function ScrollTo($variable, $id)
		{
			$ret = "$variable.scrollToElement($id, $this->Time, $this->OffsetX, $this->OffsetY";
			if (!is_null($this->Type))
			{
				$ret .= ", IScroll.utils.ease.$this->Type";
			}
			$ret .= ")";
			return $ret;
		}
	}
?>
