using Microsoft.EntityFrameworkCore;
using menu_service.Models;

namespace menu_service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between MenuCategory and MenuItem
            modelBuilder.Entity<MenuCategory>()
                .HasMany(c => c.MenuItems)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId);

            // Unique constraint on MenuCategory.Name
            modelBuilder.Entity<MenuCategory>()
                .HasIndex(c => c.Name)
                .IsUnique();
            
            // Unique constraint on MenuItem.Name
            modelBuilder.Entity<MenuItem>()
                .HasIndex(i => i.Name)
                .IsUnique();
        }
    }
}
