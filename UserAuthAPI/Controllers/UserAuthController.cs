using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserAuthAPI.Models;
using UserAuthAPI.Services;

namespace UserAuthAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserAuthController : ControllerBase
	{
		private readonly UserAuthDbContext _context;
		private readonly JwtTokenService _jwtTokenService;
		private byte[] storedHash;
		private byte[] storedSalt;
		public UserAuthController(UserAuthDbContext context, JwtTokenService jwtTokenService)
		{
			_context = context;
			_jwtTokenService = jwtTokenService;
		}

		[HttpPost("register")] // Route for registration requests
		public async Task<IActionResult> Register([FromBody] UserRegistrationModel userModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (_context.Users.Any(user => user.Username == userModel.Username || user.Email == userModel.Email))
			{
				return BadRequest("Registration failed, user already exists. Please try again.");
			}

			if (userModel.Password != userModel.ConfirmPassword)
			{
				return BadRequest("Passwords must match");
			}

			CreatePasswordHash(userModel.Password);

			if (storedHash != null && storedSalt != null)
			{
				var user = new User
				{
					Username = userModel.Username,
					Email = userModel.Email,
					PasswordHash = storedHash,
					PasswordSalt = storedSalt
				};

				_context.Users.Add(user);
				await _context.SaveChangesAsync();

			}

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

			var jwt = _jwtTokenService.GenerateJwtToken(user);

			// 3. JWT Generation

			return Ok(new { token = jwt });
		}



		public void CreatePasswordHash(string password)
		{
			using (var hmac = new HMACSHA512())
			{
				storedSalt = hmac.Key;
				storedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
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
