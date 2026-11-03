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
    public interface ISeatService
    {
        Task<ICollection<SeatDTO>> GetAllSeatsAsync(CancellationToken cancellationToken);
        Task<SeatDTO> GetSeatByIdAsync(int id, CancellationToken cancellationToken);
        Task<ICollection<SeatDTO>> GetSeatsByHallIdAsync(int hallId, CancellationToken cancellationToken);
        Task<SeatDTO> AddSeatAsync(CreateSeatDTO createSeatDTO, CancellationToken cancellationToken);
        Task UpdateSeatAsync(int id, UpdateSeatDTO updateSeatDTO, CancellationToken cancellationToken);
        Task DeleteSeatAsync(int id, CancellationToken cancellationToken);

    }
}
