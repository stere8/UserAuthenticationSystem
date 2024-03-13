using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserAuthServer.Models;
using UserAuthServer.Service;

namespace UserAuthServer
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
			builder.Services.AddCors(options =>
				options.AddPolicy(name: "MyAllowSpecificOrigins",
						builder =>
						{
							builder.WithOrigins("http://localhost:5173") // Replace with your Vue development server's origin
												 .AllowAnyHeader()
						 .AllowAnyMethod();
						}));
			
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			  .AddJwtBearer(options =>
			  {
				  options.TokenValidationParameters = new TokenValidationParameters
				  {
					  ValidateIssuerSigningKey = true,
					  IssuerSigningKey =
				new SymmetricSecurityKey(
				  Encoding.UTF8.GetBytes(builder.Configuration.GetConnectionString("JWTSecretKey"))),
					  ValidateIssuer = false,
					  ValidateAudience = false
				  };
			  });

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseCors("MyAllowSpecificOrigins");

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}