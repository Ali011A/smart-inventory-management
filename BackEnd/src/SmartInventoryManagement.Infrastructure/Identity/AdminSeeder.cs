using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventoryManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Infrastructure.Identity
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // Seed roles first 
            foreach (var role in new[] { Roles.Admin, Roles.Employee })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            //  only if no Admin exists yet
            var adminEmail = config["Seed:AdminEmail"]
                ?? throw new InvalidOperationException(
                    "Seed:AdminEmail is not configured.");
            var adminPassword = config["Seed:AdminPassword"]
                ?? throw new InvalidOperationException(
                    "Seed:AdminPassword is not configured.");

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null) return; // already seeded

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(
                    $"Failed to seed admin user: {errors}");
            }

            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
