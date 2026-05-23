using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Application.Interfaces.Services;
using SmartInventoryManagement.Infrastructure.Identity;
using SmartInventoryManagement.Infrastructure.Persistence.Context;
using SmartInventoryManagement.Infrastructure.Persistence.Repositories;
using SmartInventoryManagement.Infrastructure.Services;
using System;
using System.Security.Claims;
using System.Text;

namespace SmartInventoryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName)
            )
        );

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // 
        var jwtSecret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret not configured.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)),
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($">>> AUTH FAILED: {context.Exception.GetType().Name}: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine(">>> TOKEN VALIDATED OK");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($">>> CHALLENGE ERROR: {context.Error} | {context.ErrorDescription}");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    Console.WriteLine($">>> TOKEN RECEIVED: {context.Token?[..20]}...");
                    return Task.CompletedTask;
                }
            };
        });

        // Reposi
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}