using Microsoft.EntityFrameworkCore;

namespace UserAuthAPI.Models
{
	public class UserAuthDbContext : DbContext
	{
		public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
	}
}