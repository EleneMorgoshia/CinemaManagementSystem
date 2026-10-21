using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface ISessionRepository
    {
        Task<ICollection<Session>> GetSessions(CancellationToken cancellationToken);
        Task<Session?> GetSessionById(int id, CancellationToken cancellationToken);
        Task<ICollection<Session>> GetSessionsByMovieId(int movieId, CancellationToken cancellationToken);
        Task<ICollection<Session>> GetSessionsByHallId(int hallId, CancellationToken cancellationToken);
        Task AddSession(Session session, CancellationToken cancellationToken);
        void DeleteSession(Session session);
        void UpdateSession(Session session);
     
    }
}
