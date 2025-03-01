using Microsoft.EntityFrameworkCore;
using WelcomeService.Enums;
using WelcomeService.Models;

namespace WelcomeService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .Property(t => t.Status)
                .HasConversion<int>(); 

            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
