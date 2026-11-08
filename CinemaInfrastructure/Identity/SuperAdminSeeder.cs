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
    public class SuperAdminSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // 1. Get UserManager from Dependency Injection
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 2. Read SuperAdmin credentials from User Secrets
            var userName = configuration["SuperAdminSeed:UserName"];
            var email = configuration["SuperAdminSeed:Email"];
            var password = configuration["SuperAdminSeed:Password"];

            // 3. Validate credentials
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SuperAdmin seed credentials are missing.");


            // 4. Check whether the SuperAdmin user already exists
            var superAdmin = await userManager.FindByEmailAsync(email);

            // 5. Create SuperAdmin if the user doesn't exist
            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(superAdmin, password);

                if (!createResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ",createResult.Errors.Select(e => e.Description)));
            }

            // 6. Check whether the user already has SuperAdmin role
            if (!await userManager.IsInRoleAsync(superAdmin, "SuperAdmin"))
            {
                var roleResult = await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                if (!roleResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ",roleResult.Errors.Select(e => e.Description)));   
            }
        }
    }
}
