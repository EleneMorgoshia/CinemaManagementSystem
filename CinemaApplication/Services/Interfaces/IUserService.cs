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

namespace CinemaApplication.Services.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<UserDTO> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDTO> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<Role?> GetUserRoleByIdAsync(int id, CancellationToken cancellationToken);
        Task<Role?> GetUserRoleByEmailAsync(string email, CancellationToken cancellationToken);
        Task<UserDTO> AddUserAsync(CreateUserDTO user, CancellationToken cancellationToken);
        Task<UserDTO> UpdateUser(int id, UpdateUserDTO user, CancellationToken cancellationToken);
        Task DeleteUser(int id, CancellationToken cancellationToken);
    }
}
