using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth_service.DTO;
using auth_service.Enums;
using auth_service.Models;
using auth_service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role.Name.ToString()) 
            };
    
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
    
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
         // Register a new user
        public async Task<IdentityResult> RegisterUserAsync(Employee user, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            // Ensure Role Exists Before Assigning
            var roleEnumValue = (RoleEnum)user.RoleId;
            var roleName = roleEnumValue.ToString();

            // Check if role exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                throw new Exception($"Role {roleName} does not exist. Make sure roles are seeded.");

            // Create User
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, roleName);

            return result;
        }

        // Login a user
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Check if user exists and password is correct
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Invalid credentials");
    
            // Generate and store token in cache
            var token = await GenerateToken(user);        
            await _cache.SetStringAsync($"user_token_{user.Id}", token);
    
            return token;
        }
    
        // Get token from cache
        public async Task<string> GetTokenFromCache(int userId)
        {
            var token = await _cache.GetStringAsync($"user_token_{userId}");
            return token;
        }
    }
}   