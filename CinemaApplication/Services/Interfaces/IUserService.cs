using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Identity;
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
        Task RegistrUserAsync(RegisterDTO userDTO, CancellationToken cancellationToken = default);
        Task<ICollection<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> UpdateUserAsync(string id, UpdateUserDTO dto);
        Task DeleteUserAsync(string id);
    }
}
