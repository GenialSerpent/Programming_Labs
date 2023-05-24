using Microsoft.EntityFrameworkCore;

namespace DemoWebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Project> Projects => Set<Project>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}