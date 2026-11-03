using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Enums;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class UserManagerService : IUsermManagerService
    {
        private readonly IUserRepository _userRepository;

        public UserManagerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _userRepository.GetUsers(cancellationToken);
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.");

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            return user;
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException("Invalid email.");

            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException($"User with email {email} was not found.");

            return user;
        }

        public async Task<UserDTO> UpdateUserAsync(string id, UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.");

            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email) || !new EmailAddressAttribute().IsValid(dto.Email))
                throw new ArgumentException("Invalid user information.");

            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null && existingUser.Id != id)
                throw new ArgumentException("This email is already used by another user.");

            await _userRepository.UpdateUserAsync(id, dto);

            return await GetUserByIdAsync(id);
        }

        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.");
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
