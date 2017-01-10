<?php
	namespace BClib\Html\Tags;
	
	require_once(__DIR__ . "/_base/base_tag.php");
	
	class Bold extends \BClib\Html\Tags\_base\tag_base
	{
		public function __construct()
		{
			$this->_from_template("b");
		}
	}
?>
