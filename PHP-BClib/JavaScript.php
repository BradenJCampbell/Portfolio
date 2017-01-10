<?php
	namespace BClib;
	
	class Javascript
	{
		public static function GetElement($id, $suffix = NULL)
		{
			$ret = "document.getElementById('" . $id . "')";
			if (is_null($suffix))
			{
				return $ret;
			}
			return $ret . $suffix;
		}
		
		public static function CenterHorizontal($obj, $target)
		{
			return "$obj.style.left = AbsolutePosition($target).midX - ($obj.offsetWidth / 2)";
		}
			
		public static function PlaceBelow($top_id, $bottom_id, $distance = 0)
		{
			$ret = self::GetElement($bottom_id, ".style.position") . " = 'absolute';\n" . 
				self::GetElement($bottom_id, ".style.top") . " = " . self::GetElement($top_id, ".offsetTop") . " + " . self::GetElement($top_id, ".offsetHeight");
			if ($distance > 0)
			{
				return $ret . " + " . $distance . " + 'px'";
			}
			return $ret . " + 'px'";
		}
		
		public static function PlaceAbove($top_id, $bottom_id, $distance = 0)
		{
			$ret = self::GetElement($top_id, ".style.bottom") . " = " . self::GetElement($bottom_id, ".offsetTop");
			if ($distance > 0)
			{
				return $ret . " + " . $distance . " + 'px'";
			}
			return $ret . " + 'px'";
		}
	}
?>
