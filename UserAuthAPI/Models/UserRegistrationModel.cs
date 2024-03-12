﻿using System.ComponentModel.DataAnnotations;

namespace UserAuthAPI.Models
{
	public class UserRegistrationModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
		[Required]
		public string ConfirmPassword { get; set; }
	}
}