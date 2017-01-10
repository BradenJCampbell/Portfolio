<?php
	namespace BClib;
	class FileFetcher
	{
		private $_data;
		public function __construct($path, $extension = false)
		{
			$this->_data = self::_default();
			$this->Path = $path;
		}
		
		public function Exec()
		{
			if ($this->Path)
			{
				if ($this->ContentType)
				{
					header('Content-type: ' . $this->ContentType . '/' . $this->Extension);
				}
				if ($this->Disposition)
				{
					header('Content-Disposition: ' . $this->Disposition . '; filename="' . $this->Filename . '.' . $this->Extension . '"');
				}
				readfile($this->Path);
			}
		}
		
		public function __get($name)
		{
			if (array_key_exists($name, $this->_data))
			{
				return $this->_data[$name];
			}
		}
		
		public function __set($name, $value)
		{
			switch ($name)
			{
				case "Path":
					if (file_exists($value))
					{
						$this->_data["Path"] = $value;
						if (!$this->Extension)
						{
							$this->Extension = pathinfo($value, \PATHINFO_EXTENSION);
						}
						if (!$this->Filename)
						{
							$this->Filename = pathinfo($value, \PATHINFO_FILENAME);
						}
					}
					break;
				case "Filename":
					if (strpos($value, ".") === false)
					{
						$this->_data["Filename"] = $value;
					}
					else
					{
						$this->_data["Filename"] = pathinfo($value, \PATHINFO_FILENAME);
						$this->_data["Extension"] = pathinfo($value, \PATHINFO_EXTENSION);
					}
					break;
				default:
					if (self::_allowable($name, $value))
					{
						$this->_data[$name] = $value;
					}
					break;
			}
		}
		
		private static function _default()
		{
			return 	
			[
				"Path"        => false, 
				"ContentType" => false,
				"Extension"   => false,
				"Disposition" => false,
				"Filename"    => false
			];
		}
		
		private static function _allowable($name, $value)
		{
			if (!self::$__allowable)
			{
				self::$__allowable =
				[
					"Path" => true,
					"ContentType" => [false, "application", "image", "text"],
					"Extension" => true,
					"Disposition" => [false, "inline", "attachment"],
					"Filename" => true
				];
			}
			if (array_key_exists($name, self::$__allowable))
			{
				if (is_array(self::$__allowable[$name]))
				{
					foreach (self::$__allowable[$name] as $al)
					{
						if ($al === $value)
						{
							//  value within allowable range
							return true;
						}
					}
					//  value not in allowable range
					return false;
				}
				//  any value allowed
				return true;
			}
			//  name does not exist
			return false;
		}
		private static $__allowable;
	}
?>
