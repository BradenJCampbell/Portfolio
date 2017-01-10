<?php
	namespace BClib\Database;
	
	class db_case
	{
		public function __construct($gen, $output, $name)
		{
			$this->_gen = $gen;
			$this->_output = $output;
			$this->_name = $name;
			$this->_cases = [];
			$this->_else = NULL;
		}
		
		public function Add($condition, $value)
		{
			$this->_cases[$condition] = $value;
		}
		
		public function Els($value)
		{
			$this->_else = $value;
		}
		
		public function Make()
		{
			$this->_output->WriteLine("CASE");
			$this->_output->Indent(4);
			foreach ($this->_cases as $cond => $val)
			{
				$this->_output->Write("WHEN (");
				$this->_output->Write($cond);
				$this->_output->WriteLine(") THEN");
				$this->_output->WriteLine("    " . $val);
			}
			if (!is_null($this->_else))
			{
				$this->_output->WriteLine("ELSE");
				$this->_output->WriteLine("    " . $this->_else);
			}
			$this->_output->Unindent(4);
			$this->_output->Write("END");
		}
	}
?>
				