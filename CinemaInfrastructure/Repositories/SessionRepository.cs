using CinemaDomain.Interfaces;
using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaInfrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly CinemaDbContext _context;
        public SessionRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Session>> GetSessions(CancellationToken cancellationToken)
        {
            return await _context.Sessions.ToListAsync(cancellationToken);
        }

        public async Task<Session?> GetSessionById(int id, CancellationToken cancellationToken)
        {
            return await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        //get sessions by movie id 
        //1 movie can have several sessions
        public async Task<ICollection<Session>> GetSessionsByMovieId(int movieId, CancellationToken cancellationToken)
        {
           return await _context.Sessions.Where(s => s.MovieId == movieId).ToListAsync(cancellationToken);
        }

        //get sessions by hallid
        //1 hall can have several sessions
        public async Task<ICollection<Session>> GetSessionsByHallId(int  hallId, CancellationToken cancellationToken)
        {
            return await _context.Sessions.Where(s => s.HallId == hallId).ToListAsync(cancellationToken);
        }

        public async Task AddSession(Session session, CancellationToken cancellationToken)
        {
            await _context.Sessions.AddAsync(session, cancellationToken);
        }

        public void DeleteSession(Session session)
        {
            _context.Sessions.Remove(session);
        }

        public void UpdateSession(Session session)
        {
            _context.Sessions.Update(session);
        }
    }
}
