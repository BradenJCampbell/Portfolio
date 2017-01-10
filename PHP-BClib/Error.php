<?php
	namespace BClib;
	
	require_once(__DIR__ . "/Writer.php");
	require_once(__DIR__ . "/Structures/GetterSetter.php");
	
	class Error
	{
		public static function Settings($setting = NULL, $value = NULL)
		{
			if (is_null(self::$__settings))
			{
				self::$__settings = ["Stream" => new \BClib\Writer()];
			}
			if (!is_null($value))
			{
				switch ($setting)
				{
					case "File":
						self::$__settings["File"] = $value;
						self::$__settings["Stream"] = new \BClib\Writer(\fopen($value, "a+"));
						break;
					case "WriteDate":
						self::$__settings[$setting] = $value;
						break;
				}
			}
			if (\array_key_exists($setting, self::$__settings))
			{
				return self::$__settings[$setting];
			}
			return NULL;
		}
		public static function Handler($errno, $errstr, $errfile, $errline, array $errcontext)
		{
			self::Output($errstr, 1);
		}
		public static function Output($err, $trace_lookback = 0)
		{
			if (self::Settings("WriteDate"))
			{
				self::Settings("Stream")->Write(\date("Y-mM-d H:i:s    "));
			}
			self::Settings("Stream")->WriteLine("error: $err<br>");
			self::Settings("Stream")->Indent(4);
			foreach (self::PrintableTrace(1 + $trace_lookback) as $line)
			{
				self::Settings("Stream")->WriteLine("$line<br>");
			}
			self::Settings("Stream")->Unindent(4);
		}
		public static function CallNotFound($class, $func)
		{
			self::Output("function $class->$func not found", 1);
		}
		public static function AttributeNotFound($class, $attr)
		{
			self::Output("attribute $class->$attr not found", 1);
		}
		public static function PrintableTrace($LookBack = 0)
		{
			$trace = \debug_backtrace(\DEBUG_BACKTRACE_IGNORE_ARGS);
			for ($i = 0; $i <= $LookBack; $i++)
			{
				array_shift($trace);
			}
			$ret = [];
			foreach ($trace as $frame)
			{
				$line = "";
				if (array_key_exists('file', $frame))
				{
					$line .= $frame['file'] . " line " . $frame["line"] . ": ";
				}
				if (array_key_exists("class", $frame))
				{
					$line.= $frame["class"] . $frame["type"];
				}
				$line .= $frame["function"];
				array_push($ret, $line);
			}
			return $ret;
		}
		
		private static $__settings;
	}
	\set_error_handler("\BClib\Error::Handler", \E_ALL);
?>
