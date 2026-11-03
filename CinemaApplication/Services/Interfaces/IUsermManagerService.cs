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
    public interface IUsermManagerService
    {
        Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> UpdateUserAsync(string id, UpdateUserDTO dto);
        Task DeleteUserAsync(string id);
    }
}
