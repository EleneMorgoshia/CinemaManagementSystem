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
    public interface  ISessionService
    {
        Task<ICollection<SessionDTO>> GetSessionsAsync(CancellationToken cancellationToken);
        Task<SessionDTO> GetSessionByIdAsync(int id, CancellationToken cancellationToken);
        Task<ICollection<SessionDTO>> GetActiveSessionsAsync(CancellationToken cancellationToken);
        Task<ICollection<SessionDTO>> GetSessionsByMovieIdAsync(int movieId, CancellationToken cancellationToken);
        Task<ICollection<SessionDTO>> GetSessionsByHallIdAsync(int hallId, CancellationToken cancellationToken);
        Task<SessionDTO> AddSessionAsync(CreateSessionDTO createSessionDto, CancellationToken cancellationToken);
        Task UpdateSession(int id, UpdateSessionDTO updateSessionDTO, CancellationToken cancellationToken);
        Task DeleteSession(int id, CancellationToken cancellationToken);

    }
}
