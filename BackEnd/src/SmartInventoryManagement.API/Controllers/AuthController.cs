using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Application.DTOs.Auth;
using SmartInventoryManagement.Application.Interfaces.Services;

namespace SmartInventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
