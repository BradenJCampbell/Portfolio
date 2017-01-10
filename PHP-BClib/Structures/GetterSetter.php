<?php
	namespace BClib\Structures;
	
	require_once(__DIR__ . "/../Error.php");
	
	class GetterSetter
	{
		public function __get($name)
		{
			if (\method_exists($this, "get_" . $name))
			{
				return \call_user_func([$this, "get_" . $name], []);
			}
			\BClib\Error::Output("no getter for attribute '$name'", 1);
		}
		
		public function __set($name, $value)
		{
			if (\method_exists($this, "set_" . $name))
			{
				return \call_user_func([$this, "set_" . $name], [$value]);
			}
			\BClib\Error::Output("no setter for attribute '$name'", 1);
		}
	}
?>
