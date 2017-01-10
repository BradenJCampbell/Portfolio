<?php
	namespace BClib;
	
	require_once(__DIR__ . "/Structures/GetterSetter.php");
	
	class string_static extends \BClib\Structures\GetterSetter
	{
		public static function RemoveQuotes($str)
		{
			return \str_replace("'", "", \str_replace('"', '', $str));
		}
		
		public static function QuoteMap($str)
		{
			
		}
		
		public static function ImplodeArray($array, $delim = ", ")
		{
			return \join($delim, $array);
		}
		
		public static function ImplodeHash($hash, $delim = ", ", $pairer = " => ")
		{
			if (!\is_array($hash))
			{
				return "";
			}
			$ret = [];
			foreach ($hash as $key => $value)
			{
				if (\is_array($value))
				{
					\array_push($ret, $key . $pairer . "[" . self::ImplodeHash($value, $delim, $pairer) . "]");
				}
				else
				{
					\array_push($ret, $key . $pairer . $value);
				}
			}
			return \join($delim, $ret);
		}
			
		public static function AllOccurences($haystack, $needle)
		{
			$lastPos = 0;
			$ret = [];
			while (($lastPos = \strpos($html, $needle, $lastPos))!== false) 
			{
			    \array_push($ret, $lastPos);
			    $lastPos = $lastPos + \strlen($needle);
			}
			return $ret;
		}
	}

	class String extends string_static
	{
		public function __construct($str)
		{
			$this->_str = $str;
		}
		
		public function get_IsBlank()
		{
			return \is_null($this->_str) || $this->_str === "";
		}
		
		public function __toString()
		{
			return $this->_str;
		}
		
		private $_str;
	}
	
	class StringSubstitution
	{
		public function __construct()
		{
			$this->_subs = [];
		}
		
		public function Add($before, $after)
		{
			$this->_subs[$before] = $after;
		}
		
		public function Exec(&$str)
		{
			$ret = $str;
			foreach ($this->_subs as $before => $after)
			{
				$ret = \str_replace($before, $after, $ret);
			}
			return $ret;
		}
		
		private $_subs;
	}
	
?>
