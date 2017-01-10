<?php
	namespace BClib;
	
	class Primes
	{
		public function __construct($limit = 2)
		{
			$this->known = [2];
			$this->next = 3;
			$this->Fill($limit);
		}
		
		public function Factors($value)
		{
			$this->Fill($value);
			return $this->_fact($value, []);
		}
		
		public function IsPrime($value)
		{
			$this->Fill($value);
			return $this->_check($value);
		}
		
		public function Fill($target)
		{
			if ($target >= $this->next)
			{
				for ($i = $this->next; $i <= $target; $i++)
				{
					if ($this->_check($i))
					{
						array_push($this->known, $i);
					}
				}
				$this->next = $target + 1;
			}
		}
		
		private function _fact($value, $ret)
		{
			foreach ($this->known as $prime)
			{
				if ($value % $prime === 0)
				{
					if (array_key_exists($prime, $ret))
					{
						$ret[$prime] ++;
					}
					else
					{
						$ret[$prime] = 1;
					}
					return $this->_fact($value / $prime, $ret);
				}
			}
			return $ret;
		}
		
		private function _check($value)
		{
			foreach ($this->known as $prime)
			{
				if ($value % $prime === 0)
				{
					return false;
				}
			}
			return true;
		}
		
		private $next;
		private $known;
	}
?>
