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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IUnitOfWork unirOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unirOfWork;
            _mapper = mapper;
        }

        public async Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsers(cancellationToken);
            return users.Select(_mapper.Map<User, UserDTO>).ToList();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
                throw new ArgumentException("Id cannoe be negative or 0");
            var user = await _userRepository.GetUserById(id, cancellationToken);
            if (user == null)
                throw new ArgumentException($"There is no user with this id - {id}");
            //return EntityToDto(user);
            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(email) || email.Length > 100 || !new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException("Invalid email");
            var user = await _userRepository.GetUserByEmail(email, cancellationToken);
            if(user == null)
                throw new ArgumentException($"There is no user with this Email - {email}");
            //return EntityToDto(user);
            return _mapper.Map<User, UserDTO>(user);
        }
        public async Task<Role?> GetUserRoleByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Id cannoe be negative or 0");
            var role = await _userRepository.GetUserRoleById(id, cancellationToken);
            if (role == null)
                throw new ArgumentException($"Role for user with ID {id} was not found.");
            return role;
        }

        public async Task<Role?> GetUserRoleByEmailAsync(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 100 || !new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException("Invalid email");
            var role = await _userRepository.GetUserRoleByEmail(email, cancellationToken);
            if (role == null)
                throw new ArgumentException($"Role for user with Email {email} was not found.");
            return role;
        }

        public async Task<UserDTO> AddUserAsync(CreateUserDTO user, CancellationToken cancellationToken)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _userRepository.GetUserByEmail(user.Email, cancellationToken);
            if (existingUser == null)
                throw new ArgumentException("This email is already registered");

            var userEntity = new User 
            { 
                UserName = user.UserName,
                Email  = user.Email, 
                PasswordHash = user.Password, // todo - paswrod hash
                Role = Role.Client // ადმინის და კლიენტის ამბავი როგორ უნდა გაკონტროლდეს
            };

            await _userRepository.AddUser(userEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            //return EntityToDto(userEntity);
            return _mapper.Map<User, UserDTO>(userEntity);
        }

        public async Task<UserDTO> UpdateUser(int id, UpdateUserDTO user, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Id cannoe be negative or 0");
            if(user == null)
                throw new ArgumentNullException(nameof(user));
            var userById = await _userRepository.GetUserById(id, cancellationToken);
            if (userById == null)
                throw new ArgumentException($"There is no user with id - {id}");

            var existingUser = await _userRepository.GetUserByEmail(user.Email, cancellationToken);
            if (existingUser != null && existingUser.Id != id)
                throw new ArgumentException("This email is already used by another user");

            userById.UserName = user.UserName;
            userById.Email = user.Email;

            _userRepository.UpdateUser(userById);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            //return EntityToDto(userById);
            return _mapper.Map<User, UserDTO>(userById);
        }

        public async Task DeleteUser(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Id cannoe be negative or 0");
            var user = await _userRepository.GetUserById(id, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found");

            _userRepository.DeleteUser(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        //private UserDTO EntityToDto(User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));
        //    return new UserDTO
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Role = user.Role,
        //    };

        //}
    }
}
