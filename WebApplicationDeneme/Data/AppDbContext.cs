using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Reference> References { get; set; }
        public DbSet<WebApplicationDeneme.Models.TeamMember> TeamMembers { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
