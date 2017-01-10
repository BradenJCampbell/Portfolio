<?php
	namespace BClib;
	
	require_once (__DIR__ . "/Structures/GetterSetter.php");
	require_once (__DIR__ . "/Error.php");
	require_once (__DIR__ . "/External/PHPMailer/PHPMailerAutoload.php");
	
	class Email extends \BClib\Structures\GetterSetter
	{
		public $To;
		public $From;
		public $Subject;
		public $Message;
		public $Settings;
		public $Debug;
		
		public function __construct($To = NULL, $Subject = NULL, $Message = NULL)
		{
			$this->_errors = [];
			$this->Debug = false;
			$this->To = $To;
			$this->Subject = $Subject;
			$this->Message = $Message;
			$this->Settings = new EmailSettings();
			$this->_reply_to = NULL;
		}
		
		public function get_Errors()
		{
			return $this->_errors;
		}
		
		public function get_LastError()
		{
			if (count($this->_errors) > 0)
			{
				return $this->_errors[count($this->_errors) - 1];
			}
			return NULL;
		}
		
		public function set_ReplyTo($value)
		{
			$this->_reply_to = $value;
		}
		
		public function get_ReplyTo()
		{
			if (is_null($this->_reply_to))
			{
				return $this->From;
			}
			return $this->_reply_to;
		}

		public function Send()
		{
			$sender = new \PHPMailer;
			if ($this->From)
			{
				$sender->setFrom($this->From);
			}
			if ($this->ReplyTo)
			{
				$sender->addReplyTo($this->ReplyTo);
			}
			$sender->addAddress($this->To);
			$sender->Subject = $this->Subject;
			$sender->Body = $this->Message;
			$sender->Timeout = $this->Settings->Timeout;
			switch ($this->Settings->Mailer)
			{
				case "smtp":
					$sender->IsSMTP();
					if ($this->Debug)
					{
						$sender->SMTPDebug = 2;
						$sender->Debugoutput = 'html';
					}
					$sender->Mailer = $this->Settings->Mailer;
					$sender->Host = $this->Settings->Host;
					$sender->Port = $this->Settings->Port;
					$sender->Username = $this->Settings->Username;
					$sender->Password = $this->Settings->Password;
					$sender->SMTPAuth = true;
					$sender->SMTPSecure = false;
					if ($this->Settings->IsAuth)
					{
						$sender->SMTPSecure = $this->_con["Authentication"];
					}
					break;
			}
			if ($sender->send())
			{
				return true;
			}
			else
			{
				array_push($this->_errors, $sender->ErrorInfo);
				return false;
			}
		}
		
		private $_errors;
		private $_reply_to;
	}
	
	class EmailSettings
	{
		public function __construct()
		{
			$this->_con = ["Mailer" => NULL, "Timeout" => 5, "Port" => 25];
		}
		
		public function __get($name)
		{
			if ($name === "IsAuth")
			{
				return \array_key_exists("Authentication", $this->_con);
			}
			if (\array_key_exists($name, $this->_con))
			{
				return $this->_con[$name];
			}
			\BClib\Error::AttributeNotFound(__CLASS__, $name);
			return NULL;
		}
		
		public function Port($Port)
		{
			$this->_con["Port"] = $Port;
		}
		
		public function SMTP($Host, $Username, $Password)
		{
			$this->_con["Mailer"] = "smtp";
			$this->_con["Host"] = $Host;
			$this->_con["Username"] = $Username;
			$this->_con["Password"] = $Password;
		}
		
		public function Authentication($Authentication)
		{
			$this->_con["Authentication"] = $Authentication;
		}
	}
?>
