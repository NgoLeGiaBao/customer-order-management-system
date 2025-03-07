using auth_service.Enums;
using auth_service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace auth_service.Services
{
    public class SeedData
    {
        private readonly ILogger<SeedData> _logger;
        public SeedData(ILogger<SeedData> logger) => _logger = logger;

        public async Task Initialize(UserManager<Employee> userManager, RoleManager<Role> roleManager)
        {
            foreach (var roleName in Enum.GetNames(typeof(RoleEnum)))
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    var result = await roleManager.CreateAsync(new Role { Name = roleName });
                    _logger.Log(result.Succeeded ? LogLevel.Information : LogLevel.Error, 
                        result.Succeeded ? $"Created role: {roleName}" : $"Error creating {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var adminEmail = "admin@yourdomain.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var newAdmin = new Employee { UserName = adminEmail, Email = adminEmail, FullName = "Admin User", EmailConfirmed = true, RoleId = (int)RoleEnum.Admin };
                var createUserResult = await userManager.CreateAsync(newAdmin, "AdminPassword123!");

                if (createUserResult.Succeeded && await roleManager.FindByNameAsync(RoleEnum.Admin.ToString()) != null)
                {
                    await userManager.AddToRoleAsync(newAdmin, RoleEnum.Admin.ToString());
                    _logger.LogInformation("Admin user created and assigned to 'Admin' role.");
                }
                else
                {
                    _logger.LogError($"Error creating admin: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
