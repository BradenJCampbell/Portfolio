<?php
	namespace BClib\Html;
	
	require_once(__DIR__ . "/Attributes.php");
	require_once(__DIR__ . "/Tags/_base/base_tag.php");
	require_once(__DIR__ . "/Tags/Table.php");
	require_once(__DIR__ . "/Tags/Form.php");
	require_once(__DIR__ . "/Tags/Font.php");
	require_once(__DIR__ . "/Tags/Scroller.php");
	require_once(__DIR__ . "/Tags/ScrollPane.php");
	require_once(__DIR__ . "/Tags/Tabs.php");
	require_once(__DIR__ . "/../JavaScript.php");
	require_once(__DIR__ . "/../Writer.php");
	require_once(__DIR__ . "/../Error.php");
	
	class html_base extends \BClib\Html\Tags\_base\tag_base
	{
		public static function on_shutdown()
		{
			if (self::$__header)
			{
				foreach (self::$__header as $name => $value)
				{
					if ($value)
					{
						header($name . ": " . $value);
					}
				}
			}
			self::_knowledge()->OnLoadDefault = 0;
			//self::_head()->Script("jQuery")->Attributes["src"] = "https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js";
			//self::_head()->Script("bootstrap")->Attributes["src"] = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js";
			if (self::_has_component("menubar"))
			{
				self::_knowledge()->OnResize("//  menubar top");
				self::_knowledge()->OnResize("-MENUBAR-.style.position = 'fixed'");
				self::_knowledge()->OnResize("-MENUBAR-.style.top = '0px'");
				//  add a thin line at the base of the menubar
				self::_knowledge()->SubAdd("menubar_edge", self::_knowledge()->MenuBar("menubar")->Ender->Id);
				self::_knowledge()->OnResize("-MENUBAR_EDGE-.style.position = 'fixed'");
				self::_knowledge()->OnResize("-MENUBAR_EDGE-.style.left = 0");
				self::_knowledge()->OnResize("-MENUBAR_EDGE-.style.top = -MENUBAR-.offsetBottom");
				self::_knowledge()->OnResize("-MENUBAR_EDGE-.style.width = window.innerWidth");
			}
			$inner_height = "window.innerHeight";
			if (self::_has_component("menubar"))
			{
				$inner_height .= " - -MENUBAR-.offsetHeight";
				self::_knowledge()->OnResize("//  body top");
				self::_knowledge()->OnResize("-BODY-.style.top = -MENUBAR-.offsetTop + -MENUBAR-.offsetHeight");
			}
			if (self::_has_component("footer"))
			{
				$inner_height .= " - -FOOTER-.offsetHeight";
			}
			
			self::_knowledge()->OnResize("//  body height");
			self::_knowledge()->OnResize("-BODY-.style.height = " . $inner_height);
			self::_knowledge()->OnResize("-BODY-.style.width = (0.6 * window.innerWidth) - 25");
			if (self::_has_component("watermark"))
			{
				self::_knowledge()->OnResize("//  watermark top");
				self::_knowledge()->OnResize("-WATERMARK-.style.position = 'fixed'");
				if (self::_has_component("menubar"))
				{
					self::_knowledge()->OnResize("-WATERMARK-.style.top = -MENUBAR-.offsetTop + -MENUBAR-.offsetHeight");
				}
				self::_knowledge()->OnResize("//  watermark height");
				self::_knowledge()->OnResize("-WATERMARK-.style.height = " . $inner_height);
			}
			if (self::_has_component("footer"))
			{
				//  add a new div to the body, with the same height as the footer
				self::_component("body")->Div("body_ender")->Id = self::_id("body", "Ender");
				self::_knowledge()->SubAdd("body_ender", self::_id("body", "Ender"));
				self::_knowledge()->OnResize("//  extra spacer at end of body, same height as footer");
				self::_knowledge()->OnResize("-BODY_ENDER-.style.height = -FOOTER-.offsetHeight");
				self::_knowledge()->OnResize("//  footer top");
				self::_knowledge()->OnResize("-FOOTER-.style.position = 'fixed'");
				self::_knowledge()->OnResize("-FOOTER-.style.top = window.innerHeight - -FOOTER-.offsetHeight");
				self::_knowledge()->OnResize("-FOOTER-.style.width = window.innerWidth");
				self::_knowledge()->SubAdd("footer_edge", self::_knowledge()->MenuBar("footer")->Ender->Id);
				self::_knowledge()->OnResize("-FOOTER_EDGE-.style.position = 'fixed'");
				self::_knowledge()->OnResize("-FOOTER_EDGE-.style.left = 0");
				self::_knowledge()->OnResize("-FOOTER_EDGE-.style.top = -FOOTER-.offsetTop - -FOOTER_EDGE-.offsetHeight");
				self::_knowledge()->OnResize("-FOOTER_EDGE-.style.width = window.innerWidth");
			}
			if (self::_has_component("sidebar"))
			{
				self::_knowledge()->OnResize("-SIDEBAR-.style.top = -BODY-.style.top");
				self::_knowledge()->OnResize("-BODY-.style.left = -SIDEBAR-.style.offsetLeft + -SIDEBAR-.style.offsetWidth");
			}
			self::_head()->BeforeMake();
			self::_knowledge()->Body->BeforeMake();
			self::_js_slurp(__DIR__ . "/../Javascript/abs_pos.js");
			foreach (self::_knowledge()->Func() as $func_name)
			{
				self::_head()->Script("funcs")->Raw("function $func_name() {");
				foreach (self::_knowledge()->Func($func_name) as $func_line)
				{
					self::_head()->Script("funcs")->Raw("    " . self::_knowledge()->SubResolve($func_line) . ";");
				}
				self::_head()->Script("funcs")->Raw("}");
			}
			$on_resize = self::_knowledge()->_on_resize;
			if (count($on_resize) > 0)
			{
				self::_head()->Script("on_load")->Raw("function RefreshSizing() {");
				foreach ($on_resize as $event)
				{
					self::_head()->Script("on_load")->Raw("    " . self::_knowledge()->SubResolve($event) . ";");
				}
				self::_head()->Script("on_load")->Raw("}");
				self::_head()->Script("on_load")->Raw("window.onresize = function() { RefreshSizing(); }");
			}
			$on_load = self::_knowledge()->OnLoadMake();
			if (count($on_load) > 0)
			{
				self::_head()->Script("on_load")->Raw("window.onload = function(){");
				foreach ($on_load as $event)
				{
					self::_head()->Script("on_load")->Raw("    " . self::_knowledge()->SubResolve($event) . ";");
				}
				if (count($on_resize) > 0)
				{
					self::_head()->Script("on_load")->Raw("    RefreshSizing();");
				}
				self::_head()->Script("on_load")->Raw("}");
			}
			foreach (self::_knowledge()->Form() as $form)
			{
				self::_knowledge()->Body->Form($form)->Attributes["name"] = "'$form'";
			}
			self::_knowledge()->OnLoad("window.dispatchEvent(new Event('resize'));");
			$write = new \BClib\Writer();
			self::_head()->Make($write);
			self::_knowledge()->Body->Make($write);
			\BClib\Error::Output("took " . (microtime(true) - $_SERVER['REQUEST_TIME_FLOAT']));
		}
		
		protected static function _header($field, $value)
		{
			if (!self::$__header)
			{
				self::$__header = [];
			}
			self::$__header[$field] = $value;
		}
		
		protected static function _head()
		{
			return self::_knowledge()->Head;
		}
		
		protected static function _component($component)
		{
			return self::_knowledge()->ComponentGet($component);
		}
		
		protected static function _id($component, $suffix = NULL)
		{
			return self::_knowledge()->ComponentId($component, $suffix);
		}
				
		protected static function _has_component($component)
		{
			return self::_knowledge()->ComponentExists($component);
		}
		
		protected static function _func($name, $line)
		{
			return self::_knowledge()->Func($name, $line);
			if (is_null(self::$__funcs))
			{
				self::$__funcs = [];
			}
			if (!array_key_exists($name, self::$__funcs))
			{
				self::$__funcs[$name] = [];
			}
			\array_push(self::$__funcs[$name], $line);
		}
		
		protected static function _js_slurp($path)
		{
			return self::_knowledge()->JavascriptSlurp($path);
		}
		
		protected static function _on_resize($event, $unique = false)
		{
			return self::_knowledge()->OnResize($event, $unique);
		}
		
		private static $__header;
		private static $__head;
		private static $__expanded;
		private static $__funcs;
	}
	
	register_shutdown_function('\BClib\Html\html_base::on_shutdown');
?>
