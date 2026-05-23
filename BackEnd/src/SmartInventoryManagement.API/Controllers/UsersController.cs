using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Auth;
using SmartInventoryManagement.Application.Interfaces.Services;

namespace SmartInventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("{userId}/roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole(
            string userId, [FromBody] AssignRoleDto dto)
        {
            await _authService.AssignRoleAsync(userId, dto.RoleName);
            return NoContent();
        }
    }
}
