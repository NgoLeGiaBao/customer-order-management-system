using auth_service.DTO;
using auth_service.Models;
using auth_service.Services; 
using Microsoft.AspNetCore.Mvc;
using AuthServiceService = auth_service.Services.AuthService;

namespace auth_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly  AuthServiceService _authService;

        public AuthController(AuthServiceService authService)
        {
            _authService = authService;
        }

        // Đăng ký người dùng mới
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new Employee
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                Role = (Role)Enum.Parse(typeof(Role), registerDto.Role.ToString())
            };

            var result = await _authService.RegisterUserAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }

        // Đăng nhập và lấy token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }
    }
}