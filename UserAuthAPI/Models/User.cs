using System.ComponentModel.DataAnnotations;

namespace UserAuthAPI.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
	}
}
