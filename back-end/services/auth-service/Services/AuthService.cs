using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth_service.DTO;
using auth_service.Enums;
using auth_service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace auth_service.Services
{
    public class AuthService
    {
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        public AuthService(UserManager<Employee> userManager, RoleManager<Role> roleManager, IConfiguration config, IDistributedCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _cache = cache;
        }

        public async Task<string> GenerateToken(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null when generating token");

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new Exception("JWT Key is missing in configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Email, employee.Email)
            };

            // Lấy danh sách roles của user
            var roles = await _userManager.GetRolesAsync(employee);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> RegisterUserAsync(Employee currentUser, Employee newUser, string password)
        {
            if (currentUser == null || newUser == null)
                throw new ArgumentNullException("Current user or new user cannot be null.");

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleEnum.Admin.ToString());
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admin users can register new accounts.");

            var existingUser = await _userManager.FindByEmailAsync(newUser.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            var roleEnumValue = (RoleEnum)newUser.RoleId;
            var roleName = roleEnumValue.ToString();

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                throw new Exception($"Role {roleName} does not exist. Make sure roles are seeded.");

            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var roleAssignResult = await _userManager.AddToRoleAsync(newUser, roleName);
            if (!roleAssignResult.Succeeded)
                throw new Exception($"Failed to assign role {roleName}: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");

            return result;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = await GenerateToken(user);
            await _cache.SetStringAsync($"user_token_{user.Id}", token, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });

            return token;
        }

        public async Task<string> GetTokenFromCache(int userId)
        {
            var token = await _cache.GetStringAsync($"user_token_{userId}");
            return token ?? string.Empty;
        }
    }
}
