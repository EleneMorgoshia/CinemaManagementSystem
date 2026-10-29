using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class UserService : IUserService
    {
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task RegistrUserAsync(RegisterDTO userDTO, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {   
                UserName = userDTO.UserName,
                Email = userDTO.Email,
            };
            var result = await _userManager.CreateAsync(user, userDTO.Password); // applicationUser თავისით იზავს ჰეშიერებას
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
