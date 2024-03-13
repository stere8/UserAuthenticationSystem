using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserAuthServer.Models;

// Assuming your User model is here

namespace UserAuthServer.Service
{
	public class JwtTokenService
	{
		private readonly IConfiguration _configuration; // To access config

		public JwtTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("UserId", user.Id.ToString()) // Example: storing user's ID
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value)); // Get secret from appsettings
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(1); // Adjust token expiry as needed

			var token = new JwtSecurityToken(
				claims: claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}