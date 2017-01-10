<?php
	namespace BClib\Cgi;
	
	class Parameters
	{
		public static function Post($index)
		{
			if (array_key_exists($index, $_POST))
			{
				return $_POST[$index];
			}
			return NULL;
		}
		public static function Get($index)
		{
			if (array_key_exists($index, $_GET))
			{
				return $_GET[$index];
			}
			return NULL;
		}
		public static function Any($index)
		{
			if (array_key_exists($index, $_POST))
			{
				return $_POST[$index];
			}
			if (array_key_exists($index, $_GET))
			{
				return $_GET[$index];
			}
			return NULL;
		}
	}
?>
