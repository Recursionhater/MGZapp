using Microsoft.EntityFrameworkCore;
using WpfApp1;

namespace Shared
{
    public class AppDbContext : DbContext
    {
        private readonly IConnectionStringProvider csp;

        public AppDbContext(IConnectionStringProvider csp)
        {
            this.csp = csp;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(csp.ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(x => x.Name).HasMaxLength(200);
            modelBuilder.Entity<User>().Property(x => x.Login).HasMaxLength(30);
            modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(30);
            modelBuilder.Entity<User>().HasIndex(x => x.Login).IsUnique();
            modelBuilder.Entity<Category>().Property(x => x.Name).HasMaxLength(200);
        }

    }
}
