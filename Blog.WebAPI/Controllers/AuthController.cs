using Blog.Application.DTOs;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService _authService)
        {
            this._authService = _authService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.Token!);
            return Ok(result);
        }
        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!string.IsNullOrWhiteSpace(result.AccessToken))
            {
                return Ok(result);
            }
            return BadRequest("Email already exists");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!string.IsNullOrWhiteSpace(result.AccessToken))
            {
                return Ok(result);
            }
            return BadRequest("Please register first");
        }
    }
}
