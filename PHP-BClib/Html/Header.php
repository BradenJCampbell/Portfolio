<?php
	namespace BClib\Html;
	
	require_once(__DIR__ . "/base.php");
	
	class Header extends html_base
	{
		public static function Location($value)
		{
			self::_header("Location", $value);
		}
		
		public static function Cache($value)
		{
			if ($value)
			{
				self::_header("Cache-Control", false);
			}
			else
			{
				self::_header("Cache-Control", "no-cache");
			}
		}
	}
?>