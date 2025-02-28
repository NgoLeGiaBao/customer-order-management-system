using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using auth_service.Models;
using Microsoft.AspNetCore.Identity;
using auth_service.Enums;

namespace auth_service.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){}

        // Override the OnModelCreating method to configure the database schema
        protected override void OnModelCreating(ModelBuilder builder)
        {
             base.OnModelCreating(builder);

            // Remove AspNet prefix from table names
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName.StartsWith("AspNet"))
                    entity.SetTableName(tableName.Substring(6));
            }

            
            // Restrict deletion of related data
            builder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
