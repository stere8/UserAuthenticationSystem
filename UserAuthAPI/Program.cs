
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models;
using Microsoft.IdentityModel.Tokens;
using UserAuthAPI.Services;

namespace UserAuthAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddScoped<JwtTokenService>();
			builder.Services.AddDbContext<UserAuthDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey =
							new SymmetricSecurityKey(
								Encoding.UTF8.GetBytes(builder.Configuration.GetConnectionString("SecretKey"))),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
