using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaInfrastructure.Identity
{
    public class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider,IConfiguration configuration)
        {
            // 1. Get UserManager from Dependency Injection
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 2. Read Admin credentials from User Secrets
            var adminUserName = configuration["AdminSeed:UserName"];
            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];

            // 3. Validate credentials
            if (string.IsNullOrWhiteSpace(adminUserName) ||string.IsNullOrWhiteSpace(adminEmail) ||string.IsNullOrWhiteSpace(adminPassword))
                throw new InvalidOperationException("Admin seed credentials are missing.");

            // 4. Check whether the Admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // 5. Create Admin if the user doesn't exist
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ",createResult.Errors.Select(e => e.Description)));
                
            }

            // 6. Check whether the user already has Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

                if (!roleResult.Succeeded)
                    throw new InvalidOperationException( string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                
            }
        }
    }
}
