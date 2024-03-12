using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserAuthAPI.Models;

namespace UserAuthAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserAuthController : ControllerBase
	{
		private readonly UserAuthDbContext _context;
		public UserAuthController(UserAuthDbContext context)
		{
			_context = context;
		}

		[HttpPost("register")] // Route for registration requests
		public async Task<IActionResult> Register([FromBody] UserRegistrationModel userModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (_context.Users.Any(user => user.Username == userModel.Username))
			{
				return BadRequest("Registration failed, Username already exists. Please try again.");
			}


			if (_context.Users.Any(user => user.Email == userModel.Email))
			{
				return BadRequest("Registration failed, Email already in use. Please try again.");
			}

			CreatePasswordHash(userModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

			var user = new User
			{
				Username = userModel.Username,
				Email = userModel.Email,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok();

		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginModel userModel)
		{
			// 1. Retrieve User
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userModel.Login || u.Email == userModel.Login);
			if (user == null)
			{
				return NotFound("User not found"); // Consider security implications 
			}

			// 2. Verify Password
			if (!VerifyPasswordHash(userModel.Password, user.PasswordHash, user.PasswordSalt))
			{
				return Unauthorized("Incorrect password");
			}

			// 3. JWT Generation

			return Ok(new { token = "your_generated_jwt_token" });
		}



		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
		{
			using (var hmac = new HMACSHA512(storedSalt))
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(storedHash); 
			}
		}


	}
}
