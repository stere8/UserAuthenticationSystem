using Microsoft.EntityFrameworkCore;

namespace UserAuthServer.Models
{
	public class UserAuthDbContext : DbContext
	{
		public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
	}
}