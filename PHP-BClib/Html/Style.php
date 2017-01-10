<?php
	namespace BClib\Html;
	
	require_once(__DIR__ . "/../Error.php");
	require_once(__DIR__ . "/../Structures/TemplatedArray.php");
	require_once(__DIR__ . "/../Structures/Mapping.php");
	
	class Style extends \BClib\Structures\TemplatedArray
	{
		public function __construct()
		{
			$this->_mapping = new \BClib\Structures\Mapping();
			parent::__construct(
				"align", 
				"align-items", 
				"background", 
				"background-color", 
				"background-image", 
				"background-opacity", 
				"background-position", 
				"background-repeat", 
				"background-size", 
				"behaviour", 
				"border", 
				"border-bottom", 
				"border-collapse", 
				"border-left", 
				"border-right", 
				"border-radius", 
				"border-top", 
				"clear", 
				"color", 
				"display", 
				"flex-direction", 
				"flex-grow", 
				"float", 
				"font", 
				"font-color", 
				"font-family", 
				"font-size", 
				"font-margin", 
				"height", 
				"left", 
				"line-height", 
				"list-height", 
				"list-style", 
				"list-style-type", 
				"min-width", 
				"margin", 
				"margin-left", 
				"margin-right", 
				"opacity", 
				"overflow", 
				"overflow-x", 
				"overflow-y", 
				"padding", 
				"padding-left", 
				"padding-right", 
				"position", 
				"text-align", 
				"text-decoration", 
				"text-shadow", 
				"text-transform", 
				"top", 
				"vertical-align", 
				"white-space", 
				"width", 
				"z-index", 
				"-webkit-text-fill-color", 
				"-webkit-text-stroke-width", 
				"-webkit-text-stroke-color"
			);
			$this->_map("transform", ["-webkit-transform", "-moz-transform", "-ms-transform", "-o-transform", "transform"]);
			$this->_map("text-size-adjust", ["-webkit-text-size-adjust", "-moz-text-size-adjust", "-ms-text-size-adjust", "-o-text-size-adjust", "text-size-adjust"]);
		}
		
		public function Absorb(Style $s)
		{
			foreach ($s->Attributes as $attrib)
			{
				if (!\is_null($s[$attrib]))
				{
					$this[$attrib] = $s[$attrib];
				}
			}
		}
		
		public function BackgroundLinearGradient()
		{
			$args = \func_get_args();
			$default = \array_shift($args);
			$this["background"] = [
				$default,
				"-moz-linear-gradient(" . join(", ", $args) . ")", 
				"-webkit-linear-gradient(" . join(", ", $args) . ")", 
				"-o-linear-gradient(" . join(", ", $args) . ")",
				"linear-gradient(" . join(", ", $args) . ")"
			];
		}
		public function offsetSet($offset, $value)
		{
			$found = false;
			foreach ($this->_mapping->Get($offset) as $index)
			{
				if (parent::offsetExists($index))
				{
					parent::offsetSet($index, $value);
					$found = true;
				}
			}
			if (!$found)
			{
				\BClib\Error::Output("Style: '$offset' not found");
			}
		}
		public function offsetGet($offset)
		{
			if (parent::offsetExists($offset))
			{
				return parent::offsetGet($offset);
			}
			\BClib\Error::Output("Style: '$offset' not found");
		}
		public function Make(\BClib\Writer $writer)
		{
			$this->_make_inner($writer, NULL, $this);
		}
		private function _make_inner(\BClib\Writer $writer, $so_far, \BClib\Structures\TemplatedArray &$arr)
		{
			foreach ($arr->Attributes as $attrib)
			{
				if (is_null($so_far))
				{
					$next = $attrib;
				}
				else
				{
					$next = $so_far . "-" . $attrib;
				}
				if (!is_null($arr[$attrib]))
				{
					if ($arr[$attrib] instanceof \BClib\Structures\TemplatedArray)
					{
						$arr[$attrib]->_make_inner($writer, $next, $arr[$attrib]);
					}
					foreach((array)$arr[$attrib] as $at_arr)
					{
						$writer->Write($attrib);
						$writer->Write(":");
						$writer->Write($at_arr);
						$writer->Write(";");
					}
				}
			}			
		}
		private function _map($parent, $children)
		{
			parent::_add_attribs((array) $children);
			foreach ((array) $children as $child)
			{
				$this->_mapping->Add($parent, $child);
			}
		}
		private $_mapping;
	}
?>
