using CinemaDomain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Interfaces
{
    public interface IUserService
    {
        Task RegistrUserAsync(RegisterDTO userDTO, CancellationToken cancellationToken = default);
        Task<bool> LogInUserAsync(LoginDTO userDTO, CancellationToken cancellationToken = default);
    }
}
