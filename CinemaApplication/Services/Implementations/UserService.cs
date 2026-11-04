using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class UserService : IUserService
    {
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this._userManager = userManager;
            _mapper = mapper;
        }

        public async Task RegistrUserAsync(RegisterDTO userDTO, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(userDTO);
            var user = new ApplicationUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
            };
            var result = await _userManager.CreateAsync(user, userDTO.Password); // applicationUser თავისით იზავს ჰეშიერებას
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            
            //Assign Customer role to newly registered user
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {

                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                    throw new InvalidOperationException("Role assignment failed and user cleanup also failed.");

                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            }

        }


        //იუზერ მენეჯერის მეთოდების გადმოტანა
        public async Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID");
            
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException("Invalid email.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException($"User with email {email} was not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(string id, UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID", nameof(id));

            ArgumentNullException.ThrowIfNull(dto);
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email) || !new EmailAddressAttribute().IsValid(dto.Email))
                throw new ArgumentException("Invalid user information");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != id)
                throw new ArgumentException("This email is already used by another user");

            if (!string.Equals(user.Email, dto.Email, StringComparison.Ordinal))
                user.EmailConfirmed = false;

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<UserDTO>(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.", nameof(id));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
