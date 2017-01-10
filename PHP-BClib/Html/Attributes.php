<?php
	namespace BClib\Html;
	
	class Attributes
	{
		const COOKIE_NAME = "BClib_Html_Attributes";
		
		public static function __callStatic($name, $args)
		{
			self::_init();
			if (array_key_exists($name, self::$_data))
			{
				$index = array_shift($args);
				if (array_key_exists($index, self::$_data[$name]))
				{
					return self::$_data[$name][$index];
				}
			}
			return NULL;
		}
		
		public static function ServerPath($filepath)
		{
			return \realpath(\str_replace(self::Server("Root"), '', \realpath($filepath)));
		}
		
		private static function _init()
		{
			if (!self::$_data)
			{
				self::$_data =
				[
					"Client" => ["IP" => self::_client_ip()],
					"Server" => ["Root" => \realpath($_SERVER['DOCUMENT_ROOT']), "FilePath" => \realpath($_SERVER['DOCUMENT_ROOT'] . $_SERVER['PHP_SELF'])], 
					"Page"   => ["width" => 640, "height" => 480]
				];
			}
		}
		private static function _client_ip()
		{
			foreach (['HTTP_CLIENT_IP', 'HTTP_X_FORWARDED_FOR', 'HTTP_X_FORWARDED', 'HTTP_FORWARDED_FOR', 'HTTP_FORWARDED', 'REMOTE_ADDR'] as $env)
			{
				if (getenv($env))
				{
					return getenv($env);
				}
			}
			return NULL;
		}
		
		private static $_data;
	}
?>
