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

        public async Task<UserDTO> UpdateUserAsync(string currentUserId, string id, UpdateUserDTO dto)
        {  
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID", nameof(id));
          
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found");

            await ValidateUpdatePermissionAsync(currentUserId, user);
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email) || !new EmailAddressAttribute().IsValid(dto.Email))
                throw new ArgumentException("Invalid user information.");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != id)
                throw new ArgumentException("This email is already used by another user.");

            if (!string.Equals(user.Email, dto.Email, StringComparison.Ordinal))
                user.EmailConfirmed = false;

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<UserDTO>(user);
        }

        public async Task DeleteUserAsync(string currentUserId, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.", nameof(id));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            await ValidateDeletePermissionAsync(currentUserId, user);
 
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task MakeAdminAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} was not found.");

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                throw new InvalidOperationException("Cannot change the roles of a SuperAdmin.");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                throw new InvalidOperationException("User is already an admin.");

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        public async Task RemoveAdminRoleAsync(string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} was not found.");

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                throw new InvalidOperationException("Cannot change the roles of a SuperAdmin.");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                throw new InvalidOperationException("User is not an Admin.");

            //ეს იუზერი გავხადოთ ქოსთუმერის როლის როცა ადმინობას წავართმევთ
            if (!await _userManager.IsInRoleAsync(user, "Customer"))
            {
                var customerResult = await _userManager.AddToRoleAsync(user, "Customer");

                if (!customerResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ",customerResult.Errors.Select(e => e.Description)));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private async Task ValidateUpdatePermissionAsync(string currentUserId, ApplicationUser targetUser)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("Current user ID is missing.");

            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null)
                throw new UnauthorizedAccessException("Current account was not found.");

            if (currentUserId == targetUser.Id)
                return;

            if (await _userManager.IsInRoleAsync(targetUser, "SuperAdmin"))
                throw new UnauthorizedAccessException("Cannot update another SuperAdmin.");

            if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin"))
                return;

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var targetIsAdmin = await _userManager.IsInRoleAsync(targetUser, "Admin");
            var targetIsCustomer = await _userManager.IsInRoleAsync(targetUser, "Customer");

            if (isAdmin && targetIsCustomer && !targetIsAdmin)
                return;

            throw new UnauthorizedAccessException("You do not have permission to update this user.");
        }
        private async Task ValidateDeletePermissionAsync(string currentUserId,ApplicationUser targetUser)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("Current user ID is missing.");

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                throw new UnauthorizedAccessException("Current account was not found.");

            //Nobody can delete SuperAdmin.
            if (await _userManager.IsInRoleAsync(targetUser, "SuperAdmin"))
                throw new UnauthorizedAccessException("A SuperAdmin account cannot be deleted.");

            // SuperAdmin can delete Admins and Customers.
            if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin"))
                return;

            // Other callers cannot delete an Admin, including themselves.
            if (await _userManager.IsInRoleAsync(targetUser, "Admin"))
                throw new UnauthorizedAccessException("Only SuperAdmin can delete an Admin.");

            var targetIsCustomer =await _userManager.IsInRoleAsync(targetUser, "Customer");
            var isAdmin =await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Customers can delete themselves; Admins can delete Customers.
            if (targetIsCustomer && (currentUserId == targetUser.Id || isAdmin))
                return;
            throw new UnauthorizedAccessException("You do not have permission to delete this user.");
        }
    
        public async Task<ICollection<string>> GetUserRoleByIdAsync(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.", nameof(id));

            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            return await _userManager.GetRolesAsync(user);
        }
    }
}
