<?php
	namespace BClib;
	
	function regex($patterns_arr, $target)
	{
		$ret = [];
		foreach ((array)$patterns_arr as $p)
		{
			$matches = [];
			if(preg_match($p, $target, $matches, PREG_OFFSET_CAPTURE))
			{
				foreach ($matches as $m)
				{
					$ret[$m[1]] = $m[0];
				}
			}
		}
		return $ret;
	}
	function regex_all($patterns_arr, $target)
	{
		$ret = [];
		foreach ((array)$patterns_arr as $p)
		{
			$matches = [];
			if(preg_match_all($p, $target, $matches, PREG_OFFSET_CAPTURE))
			{
				foreach ($matches[0] as $m)
				{
					$ret[$m[1]] = $m[0];
				}
			}
		}
		return $ret;
	}
?>
