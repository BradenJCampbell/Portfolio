<?php
	namespace BClib\Html;
	
	require_once(__DIR__ . "/Cgi.php");
	require_once(__DIR__ . "/Html/Header.php");
	require_once(__DIR__ . "/Html/base.php");
	require_once(__DIR__ . "/Html/MenuBar.php");
						
	class Head extends html_base
	{
		public static function __callStatic($name, $args)
		{
			return self::_head()->__call($name, $args);
		}
		
		public static function Style($type = "default")
		{
			switch ($type)
			{
				case "default":
					return self::_knowledge()->Style("default", "html *");
					break;
				case "page":
					$comps = [];
					foreach (["header", "body", "footer"] as $comp)
					{
						\array_push($comps, "#" . self::_knowledge()->ComponentId($comp));
					}							
					return self::_knowledge()->Style("page", join(", ", $comps));
					break;
			}
		}
	}		
	
	class Scripts extends html_base
	{
		public static function OnLoad($script)
		{
			self::_knowledge()->OnLoad($script);
		}
		
		public static function OnResize($script)
		{
			self::_knowledge()->OnResize($script);
		}
		
		public static function Slurp($path)
		{
			self::_js_slurp($path);
		}
	}
	
	class Body extends html_base
	{
		public static function __callStatic($name, $args)
		{
			if (\method_exists(self::_component("body"), $name))
			{
				return \call_user_func([self::_component("body"), $name], $args);
			}
			return self::_component("body")->__call($name, $args);
		}
		
		public static function Style()
		{
			return self::_component("body")->Style;
		}
	}
	
	class Watermark extends html_base
	{
		public static function __callStatic($name, $args)
		{
			return self::_component("watermark")->__call($name, $args);
		}
		
		public static function Style()
		{
			return self::_component("watermark")->Style;
		}
	}
	
	class Footer extends html_base
	{
		public static function Style($type = "background")
		{
			return self::_bar()->Style($type);
		}
		
		public static function Cell($index = NULL)
		{
			return self::_bar()->Cell($index);
		}
		
		private static function _bar()
		{
			return self::_knowledge()->MenuBar("footer");
		}
	}
	
	class MenuBar extends html_base
	{
		public static function Style($type = "background")
		{
			return self::_bar()->Style($type);
		}
		
		public static function Cell($index = NULL)
		{
			return self::_bar()->Cell($index);
		}
		
		private static function _bar()
		{
			return self::_knowledge()->MenuBar("menubar");
		}
	}
?>
