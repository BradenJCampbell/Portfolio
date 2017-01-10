<?php
	namespace BClib;
	
	class Writer
	{
		private $stream;
		private $indent;
		private $needs_indent;
		public function __construct($stream = false)
		{
			$this->stream = $stream;
			$this->indent = 0;
			$this->needs_indent = false;
		}
		public function Indent($spaces)
		{
			$this->indent += $spaces;
			if ($this->indent < 0)
			{
				$this->indent = 0;
			}
		}
		public function Unindent($spaces)
		{
			$this->indent -= $spaces;
			if ($this->indent < 0)
			{
				$this->indent = 0;
			}
		}
		public function Write($str)
		{
			if ($this->needs_indent)
			{
				for ($i = 0; $i < $this->indent; $i++)
				{
					$this->_write(" ");
				}
				$this->needs_indent = false;
			}
			$this->_write($str);
		}
		public function WriteLine($str = false)
		{
			if ($str)
			{
				$this->Write($str);
			}
			$this->Write("\n");
			$this->needs_indent = true;
		}
		public function WriteDelim($str = false)
		{
			if ($str)
			{
				$this->WriteLine($str);
			}
			$this->WriteLine("$$");
		}
		public function __destruct()
		{
			if ($this->stream)
			{
				\fclose($this->stream);
			}
		}
		private function _write($str)
		{
			if ($this->stream)
			{
				fwrite($this->stream, $str);
			}
			else
			{
				echo $str;
			}
		}
	}
?>
