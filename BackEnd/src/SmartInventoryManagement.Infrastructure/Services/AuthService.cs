using Microsoft.AspNetCore.Identity;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Auth;
using SmartInventoryManagement.Application.Interfaces.Services;
using SmartInventoryManagement.Domain.Exceptions;
using SmartInventoryManagement.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
      
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ConflictException("A user with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,   
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }

           
            await _userManager.AddToRoleAsync(user, Roles.Employee);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(
     user.Id,
     user.Email!,
     roles
 );

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

        
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new UnauthorizedException("Invalid credentials.");

            
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException(
                    "Account is temporarily locked. Please try again later.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(
    user.Id,
    user.Email!,
    roles
);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task AssignRoleAsync(string userId, string roleName)
        {
            
            if (roleName != Roles.Admin && roleName != Roles.Employee)
                throw new ValidationException(
                    $"Invalid role. Allowed values: {Roles.Admin}, {Roles.Employee}");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var currentRoles = await _userManager.GetRolesAsync(user);

            
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }
        }
    }
}
