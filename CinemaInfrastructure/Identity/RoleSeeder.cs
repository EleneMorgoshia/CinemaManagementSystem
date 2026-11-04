using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaInfrastructure.Identity
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync
            (IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.
                GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[]
            { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
