using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Interfaces
{
    public interface IHallService
    {
        Task<ICollection<HallDTO>> GetAllHallsAsync(CancellationToken cancellationToken);
        Task<HallDTO> GetHallByIdAsync(int id, CancellationToken cancellationToken);
        Task<HallDTO> AddHallAsync(CreateHallDTO createHallDTO, CancellationToken cancellationToken);
        Task UpdateHallAsync(int id, UpdateHallDTO updateHallDTO, CancellationToken cancellationToken);
        Task DeleteHallAsync(int id, CancellationToken cancellationToken);

    }
}
