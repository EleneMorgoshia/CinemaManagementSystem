using CinemaDomain.Enums;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<UserDTO>> GetUsers( CancellationToken cancellationToken);
        Task<UserDTO?> GetUserByIdAsync(string userId);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(string userId, UpdateUserDTO dto);
        Task DeleteUserAsync(string userId);
    }
}
