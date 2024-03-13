using System.ComponentModel.DataAnnotations;

namespace UserAuthServer.Models
{
	public class UserLoginModel
	{
		[Required]
		public string Login { get; set; } 

		[Required]
		public string Password { get; set; }
	}
}