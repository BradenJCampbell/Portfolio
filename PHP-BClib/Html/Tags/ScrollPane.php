<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	require_once(__DIR__ . "/../../String.php");
	
	class ScrollPane extends \BClib\Html\Tags\_base\tag_base
	{
		public $RefreshRate;
		public $UpArrow;
		public $DownArrow;
		
		public function __construct()
		{
			$this->_from_template("Div");
			$this->_sb_content = $this->_get_contents("Div")["__content"];
			$this->_scroll_up = $this->_sb_content->_get_contents("Div")["__scroll_up"];
			$this->_scroll_down = $this->_sb_content->_get_contents("Div")["__scroll_down"];
			$this->_arrow_style = new \BClib\Html\Style();
			$this->_arrow_style["height"] = "20px";
			$this->_arrow_style["background-size"] = "contain";
			$this->_arrow_style["background-repeat"] = "no-repeat";
			$this->_arrow_style["background-position"] = "right";
			$this->RefreshRate = 100;
		}
		
		public function get__ArrowStyle()
		{
			return $this->_arrow_style;
		}
			
		public function get__Content()
		{
			return $this->_sb_content;
		}
		
		public function set__Id($value)
		{
			parent::set__Id($value);
			$this->_sb_content->Id = \BClib\String::RemoveQuotes($value) . "_content";
			$this->_scroll_up->Id = \BClib\String::RemoveQuotes($value) . "_up";
			$this->_scroll_down->Id = \BClib\String::RemoveQuotes($value) . "_down";
		}
		
		public function __call($name, $args)
		{
			$this->_sb_content->__call($name, $args);
		}
		
		public function BeforeMake()
		{
			if (\is_null(self::$_deployed_global))
			{
				self::_script("function ScrollPane_HideScrollbars(box, target, idealHeight, idealWidth) {");
				self::_script("    if (!box || !target) {");
				self::_script("        return;");
				self::_script("    }");
				self::_script("    var scroll_bar_width = target.offsetWidth - target.clientWidth;");
				self::_script("    var scroll_bar_height = target.offsetHeight - target.clientHeight;");
				self::_script("    if (!idealHeight && box.offsetHeight) {");
				self::_script("        idealHeight = box.offsetHeight;");
				self::_script("    }");
				self::_script("    if (!idealHeight && target.offsetHeight) {");
				self::_script("        idealHeight = target.offsetHeight - scroll_bar_height;");
				self::_script("    }");
				self::_script("    if (!idealWidth && box.offsetWidth) {");
				self::_script("        idealWidth = box.offsetWidth;");
				self::_script("    }");
				self::_script("    if (!idealWidth && target.offsetWidth) {");
				self::_script("        idealWidth = target.offsetWidth - scroll_bar_width;");
				self::_script("    }");
				self::_script("    target.style.width = idealWidth + scroll_bar_width;");
				self::_script("    target.style.height = idealHeight + scroll_bar_height;");
				self::_script("    target.style.overflow = 'auto';");
				self::_script("    box.style.width = idealWidth;");
				self::_script("    box.style.height = idealHeight;");
				self::_script("    box.style.overflow = 'hidden';");
				//self::_script("    console.log(target.id + ' bars hidden');");  //  DEBUG
				self::_script("}");
				self::_script("function ScrollPane_VerticalScroll(box, target, up, down) {");
				self::_script("    if (!box || !target || !up || !down) {");
				self::_script("        return;");
				self::_script("    }");
				self::_script("    ScrollPane_HideScrollbars(box, target);");
				self::_script("    if (target.scrollTop > 0) {");
				self::_script("        up.style.display = 'block';");
				self::_script("    } else {");
				self::_script("        up.style.display = 'none';");
				self::_script("    }");
				self::_script("    if (target.scrollTop < target.scrollHeight - target.offsetHeight) {");
				self::_script("         down.style.display = 'block';");
				self::_script("    } else {");
				self::_script("        down.style.display = 'none';");
				self::_script("    }");
				self::_script("    up.style.position = 'absolute';");
				self::_script("    up.style.width = box.offsetWidth;");
				self::_script("    up.style.left = box.offsetLeft;");
				self::_script("    up.style.top = box.offsetTop;");
				self::_script("    down.style.position = 'absolute';");
				self::_script("    down.style.width = box.offsetWidth;");
				self::_script("    down.style.left = box.offsetLeft;");
				self::_script("    down.style.top = box.offsetTop + box.offsetHeight - down.offsetHeight;");
				self::_script("}");
				self::$_deployed_global = true;
			}
			$box_id     = \BClib\String::RemoveQuotes($this->Id);
			$content_id = \BClib\String::RemoveQuotes($this->_sb_content->Id);
			$up_id      = \BClib\String::RemoveQuotes($this->_scroll_up->Id);
			$down_id    = \BClib\String::RemoveQuotes($this->_scroll_down->Id);
			$hide_func = $box_id . "_HideScrollbar";
			$scroll_func = $box_id . "_ScrollVertical";
			self::_script("function $scroll_func()");
			self::_script("{");
			self::_script("    ScrollPane_VerticalScroll(");
			self::_script("        document.getElementById('$box_id'),");
			self::_script("        document.getElementById('$content_id'),");
			self::_script("        document.getElementById('$up_id'),");
			self::_script("        document.getElementById('$down_id')");
			self::_script("    );");
			//self::_script("    alert($box_id.id + ' bar fiddle');");  //  DEBUG
			self::_script("    window.setTimeout($scroll_func, $this->RefreshRate);");
			self::_script("}");
			self::_knowledge()->OnLoad("window.setTimeout(function() {");
			self::_knowledge()->OnLoad("    $scroll_func();");
			self::_knowledge()->OnLoad("}, 0);");
			$this->_scroll_up->Style->Absorb($this->_arrow_style);
			$this->_scroll_down->Style->Absorb($this->_arrow_style);
			$this->_scroll_up->Style["background-image"] = "url($this->UpArrow)";
			$this->_scroll_down->Style["background-image"] = "url($this->DownArrow)";
		}
		
		private static function _script($line)
		{
			self::_knowledge()->Head->Script("ScrollPane")->Raw($line);
		}
		private $_sb_content;
		private $_scroll_up;
		private $_scroll_down;
		private $_arrow_style;
		private static $_deployed_global;
	}
?>
