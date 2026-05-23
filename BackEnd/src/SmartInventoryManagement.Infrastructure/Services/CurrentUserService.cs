using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.Interfaces.Services;
using SmartInventoryManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace SmartInventoryManagement.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("User identity not found.");

        public string Email =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedException("User email not found.");

        public bool IsAdmin =>
            _httpContextAccessor.HttpContext?.User
                .IsInRole(Roles.Admin) ?? false;
    }
}
