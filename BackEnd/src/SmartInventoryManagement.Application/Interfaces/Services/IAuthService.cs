using SmartInventoryManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto requestDto);
        Task AssignRoleAsync(string userId, string roleName);
    } 
}
