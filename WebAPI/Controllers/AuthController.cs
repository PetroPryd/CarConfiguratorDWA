using CarConfigurator.Business.DTOs.Auth;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (result, error) = await _authService.RegisterAsync(dto);
            if (error != null) return BadRequest(error);

            return Ok(result);
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (result, error) = await _authService.LoginAsync(dto);
            if (error != null) return Unauthorized(error);

            return Ok(result);
        }

        // POST /api/auth/changepassword
        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null) return Unauthorized();

            var (ok, error) = await _authService.ChangePasswordAsync(username, dto);
            if (!ok && error == null) return NotFound("User not found.");
            if (error != null) return BadRequest(error);

            return NoContent();
        }
    }
}
