<?php
	namespace BClib\Database;
	
	require_once(__DIR__ . "/db_common.php");
	
	class db_function
	{
		private $gen;
		private $output;
		public $Name;
		public $ReturnType;
		private $params;
		private $defs;
		private $stmts;
		private $_exists;
		private $parent;
		private $_if;
		
		public function __construct($gen, $output, $schema, $name)
		{
			$this->gen = $gen;
			$this->output = $output;
			$this->parent = $schema;
			$this->Name = $name;
			$this->defs = [];
			$this->params = [];
			$this->stmts = [];
			$this->ReturnType = false;
			$this->_if = 0;
		}
		public function __get($name)
		{
			switch($name)
			{
				case "Schema":
					return $this->parent->Name;
					break;
				case "FullName":
					return "`$this->Schema`.`$this->Name`";
					break;
			}
		}
		public function Parameter($name, $type)
		{
			$this->params[$name] = $type;
			$this->_exists = true;
		}
		public function Definition($name, $type)
		{
			$this->defs[$name] = $type;
			$this->_exists = true;
		}
		public function Statement($stmt)
		{
			$new_stmt = [];
			for ($i = 0; $i <= $this->_if; $i++)
			{
				array_push($new_stmt, "    ");
			}
			array_push($new_stmt, $stmt);
			array_push($this->stmts, $new_stmt);
			$this->_exists = true;
		}
		/*
		public function NewIf($condition)
		{
			$this->Statement("IF (" . $condition . ") THEN");
			$this->_if ++;
		}
		public function ElsIf($condition)
		{
			if ($this->_if > 0)
			{
				$this->_if --;
				$this->Statement("ELSEIF (" . $condition . ") THEN");
				$this->_if ++;
			}
		}
		public function EndIf()
		{
			if ($this->_if > 0)
			{
				$this->_if --;
				$this->Statement("END IF;");
			}
		}
		*/
		public function MakeCall($args_arr = NULL)
		{
			$args = [];
			if (is_array($args_arr))
			{
				foreach ($this->params as $p_name => $p_type)
				{
					if (array_key_exists($p_name, $args_arr))
					{
						array_push($args, $args_arr[$p_name]);
					}
					else
					{
						array_push($args, "NULL");
					}
				}
			}
			$ret = "";
			if ($this->ReturnType)
			{
				$ret .= "SELECT";
			}
			else
			{
				$ret .= "CALL";
			}
			$ret .= " `$this->Schema`.`$this->Name`(" . join(", ", $args) . ");";
			return $ret;
		}
		public function Make()
		{
			$this->output->Write("CREATE DEFINER=`root`@`localhost` ");
			if ($this->ReturnType)
			{
				$this->output->Write("FUNCTION");
			}
			else
			{
				$this->output->Write("PROCEDURE");
			}
			$this->output->Write(" `$this->Schema`.`$this->Name`(");
			$params = [];
			foreach ($this->params as $p_name => $p_type)
			{
				array_push($params, "$p_name $p_type");
			}
			$this->output->Write(join(", ", $params));
			$this->output->Write(")");
			if ($this->ReturnType)
			{
				$this->output->Write(" RETURNS $this->ReturnType READS SQL DATA");
			}
			$this->output->WriteLine();
			$this->output->WriteLine("BEGIN");
			foreach ($this->defs as $d_name => $d_type)
			{
				$this->output->WriteLine("    DECLARE $d_name $d_type;");
			}
			foreach ($this->stmts as $stmt)
			{
				foreach ((array)$stmt as $stmt_fragment)
				{
					$this->output->Write($stmt_fragment);
				}
				$this->output->WriteLine();
			}
			$this->output->WriteLine("END");
			$this->output->WriteDelim();
		}
	}
?>
