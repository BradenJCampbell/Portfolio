<?php
	namespace BClib\Html\Tags\_base;
	
	require_once(__DIR__ . "/../../../Structures/GetterSetter.php");
	require_once(__DIR__ . "/../../Style.php");
	require_once(__DIR__ . "/../../../JavaScript.php");
	require_once(__DIR__ . "/../../../Error.php");
	require_once(__DIR__ . "/../../../String.php");
	require_once(__DIR__ . "/base_tag.php");
	
	class tag_base_knowledge extends \BClib\Html\Tags\_base\tag_base
	{
		public $OnLoadDefault;
		public $Head;
		public $Body;
		public $Subs;
		public $CommonAttributes;
		
		public function __construct()
		{
			
			$this->OnLoadDefault = 1;
			$this->_on_load = [];
			$this->_on_resize = [];
			$this->CommonAttributes =
				[
					"id", 
					"class", 
					"dir", 
					"lang", 
					"style", 
					"title", 
					"onblur", 
					"onchange", 
					"onclick", 
					"ondblclick", 
					"onfocus", 
					"onkeydown", 
					"onkeypress", 
					"onkeyup", 
					"onload", 
					"onmousedown", 
					"onmousemove", 
					"onmouseout", 
					"onmouseover",
					"onmouseup",
					"onreset",
					"onselect",
					"onsubmit",
					"onunload"
				];
			$this->Head = new \BClib\Html\Tags\_base\tag_base("head", false, []);
			$this->Body = new \BClib\Html\Tags\_base\tag_base("body", false, $this->CommonAttributes);
			$this->_components = ["body" => $this->Body];
			$this->Subs = new \BClib\StringSubstitution();
			$this->Subs->Add("-BODY-", \BClib\Javascript::GetElement($this->ComponentID("body")));
			$this->_styles = [];
			$this->_funcs = [];
			$this->_js_slurp = [];
			$this->_forms = [];
			$this->_func_objs = [];
			$this->_templates = [];
			$this->_reflect = [];
			$this->_menubars = [];
			//$reflector = new \BClib\Reflection(\get_parent_class(__CLASS__));
		}
		
		public function TagTemplate($name, $is_atomic = false, $attribs = [])
		{
			if (!\array_key_exists($name, $this->_templates))
			{
				$this->_templates[$name] = ["is_atomic" => $is_atomic, "attribs" => $attribs];
			}
		}
		public function TagCreate($name)
		{
			if (array_key_exists($name, $this->_templates))
			{
				return new tag_base($name, $this->_templates["is_atomic"], \array_merge($this->CommonAttributes, $this->_templates["attribs"]));
			}
			\BClib\Error::Output("Tag template '$name' not found");
		}
		public function Func($name = NULL, $code = NULL)
		{
			if (is_null($name))
			{
				return \array_keys($this->_funcs);
			}
			if (!is_null($code))
			{
				if (!\array_key_exists($name, $this->_funcs))
				{
					$this->_funcs[$name] = [];
				}
				\array_push($this->_funcs[$name], $code);
			}
			if (\array_key_exists($name, $this->_funcs))
			{
				return $this->_funcs[$name];
			}
			return NULL;
		}
		
		public function MenuBar($name)
		{
			if (!\array_key_exists($name, $this->_menubars))
			{
				$this->_menubars[$name] = new \BClib\Html\menubar_bar($name);
			}
			return $this->_menubars[$name];
		}
		
		public function ComponentId($component, $suffix = NULL)
		{
			$ret = "Main_" . $component;
			if (is_null($suffix))
			{
				return $ret;
			}
			return $ret . "_" . $suffix;
		}
		
		public function ComponentExists($component)
		{
			return \array_key_exists($component, $this->_components);
		}
		
		public function SubAdd($component, $id = NULL)
		{
			if (is_null($id))
			{
				$id = $component;
			}
			$this->Subs->Add("-" . \strtoupper($component) . "-", \BClib\Javascript::GetElement($id));
		}
		
		public function SubResolve(&$str)
		{
			return $this->Subs->Exec($str);
		}
		
		public function Components()
		{
			return \array_keys($this->_components);
		}	
		
		public function ComponentGet($component)
		{
			if (!$this->ComponentExists($component))
			{
				$this->_expand();
				$new = $this->Body->Div($this->ComponentId($component));
				$new->Id = $this->ComponentId($component);
				switch($component)
				{
					case "watermark":
						$new->Style["z-index"] = "1";
						$new->Style["text-align"] = "center";
						break;
					case "footer":
						$this->Style("footer", "#" . $this->ComponentId("footer"));
					case "menubar":
						$new->Style["left"] = "0px";
						$new->Style["width"] = "100%";
						$new->Style["z-index"] = "3";
						break;
				}
				$this->_components[$component] = $new;
				$this->SubAdd($component, $this->ComponentId($component));
			}
			return $this->_components[$component];
		}
		
		public function OnResize($event, $unique = false)
		{
			if ($unique)
			{
				$this->_on_resize[$event] = $event;
			}
			else
			{
				array_push($this->_on_resize, $event);
			}
		}
			
		public function OnLoad($event, $priority = -1, $unique = false)
		{
			if ($priority < 0)
			{
				$priority = $this->OnLoadDefault;
			}
			if (!\array_key_exists($priority, $this->_on_load))
			{
				$this->_on_load[$priority] = [];
			}
			if ($unique)
			{
				$this->_on_load[$priority][$event] = $event;
			}
			else
			{
				array_push($this->_on_load[$priority], $event);
			}
		}
		
		public function OnLoadMake()
		{
			$ret = [];
			$keys = \array_keys($this->_on_load);
			\sort($keys);
			foreach ($keys as $priority)
			{
				foreach ($this->_on_load[$priority] as $event)
				{
					\array_push($ret, $this->SubResolve($event));
				}
			}
			return $ret;
		}

		public function JavascriptSlurp($path)
		{
			if (!array_key_exists($path, $this->_js_slurp))
			{
				if (\file_exists($path))
				{
					foreach (\explode("\n", \file_get_contents($path)) as $line)
					{
						$this->Head->Script("slurp")->Raw($line);
					}
				}
				$this->_js_slurp[$path] = true;
			}
		}

		public function StyleExists($name)
		{
			return \array_key_exists($this->_style_name($name), $this->_styles);
		}
		
		public function Style($name, $prefix = NULL)
		{
			$name = $this->_style_name($name);
			if (!$this->StyleExists($name) && !is_null($prefix))
			{
				$this->_styles[$name] = new \BClib\Html\Style();
				$this->Head->Style("knowledge")->Raw($prefix . " {");
				$this->Head->Style("knowledge")->Raw($this->_styles[$name]);
				$this->Head->Style("knowledge")->Raw("}");
			}
			if ($this->StyleExists($name))
			{
				return $this->_styles[$name];
			}
			return NULL;
		}
		
		public function Form($name = NULL)
		{
			if (is_null($name))
			{
				return \array_keys($this->_forms);
			}
			$this->_forms[$name] = true;
		}
		
		private function _style_name($name)
		{
			if (is_array($name))
			{
				foreach ($name as $ind => $data)
				{
					if (is_null($data))
					{
						unset($name[$ind]);
					}
				}
				return join("_", $name);
			}
			return $name;
		}
		
		private function _expand()
		{
			if (count($this->_components) <= 1)
			{
				$old_body = $this->Body;
				$this->Body = new \BClib\Html\Tags\_base\tag_base("body", false, []);
				$this->Body->Style["left"] = "0px";
				$this->Body->Style["padding"] = "0px";
				$this->_components["body"] = $this->Body->Div($this->ComponentId("body"));
					$this->_components["body"]->Id = $this->ComponentId("body");
					$this->_components["body"]->Style["position"] = "relative";
					$this->_components["body"]->Style["z-index"] = "2";
					$this->_components["body"]->Style["width"] = "100%";
					$this->_components["body"]->Style["left"] = "0px";
					$this->_components["body"]->_contents = $old_body->_contents;
			}
		}
		private $_on_load;
		public $_on_resize;
		private $_funcs;
		private $_components;
		private $_styles;
		private $_js_slurp;
		private $_forms;
		private $_func_objs;
		private $_templates;
		private $_reflect;
		private $_menubars;
	}
	
	class knowledge_func extends \BClib\Structures\GetterSetter
	{
		public $Declaration;

		public function __construct($declaration)
		{
			$this->Declaration = $declaration;
			$this->_code = [];
			$this->Priority = 0;
		}
		
		public function get_Priority()
		{
			return $this->_priority;
		}
		
		public function set_Priority($value)
		{
			if (!array_key_exists($value, $this->_code))
			{
				$this->_code[$value] = [];
			}
			$this->_priority = $value;
		}
		
		public function Add($code)
		{
			array_push($this->_code[$this->_priority], $code);
		}
		
		public function Deploy($tag)
		{
			$tag->Raw($this->Declaration . " {");
			$keys = \array_keys($this->_on_load);
			\sort($keys);
			foreach ($keys as $priority)
			{
				foreach ($this->_on_load[$priority] as $event)
				{
					$tag->Raw("    $event;");
				}
			}
			$tag->Raw("}");
		}
	}
?>
