using auth_service.DTO;
using auth_service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.Controllers
{
    [Route("auth-service")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly auth_service.Services.AuthService _authService;
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthController(auth_service.Services.AuthService authService, UserManager<Employee> userManager, RoleManager<Role> roleManager)
        {
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var newUser = new Employee
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                RoleId = model.RoleId
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign role using RoleManager
            if (!string.IsNullOrEmpty(model.RoleId.ToString()))
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                if (role == null)
                    return BadRequest("Invalid Role ID");
                await _userManager.AddToRoleAsync(newUser, role.Name);
            }

            return Ok("User registered successfully.");
        }

        // Sign in and get a token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
