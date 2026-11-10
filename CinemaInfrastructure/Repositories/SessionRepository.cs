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
            return await _context.Sessions.Include(s => s.Movie).Include(s => s.Hall).ToListAsync(cancellationToken);
        }
        public async Task<ICollection<Session>> GetActiveSessions(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            return await _context.Sessions
                .Where(session => session.Status && session.StartsAt > now)
                .Include(session => session.Movie)
                .Include(session => session.Hall)
                .ToListAsync(cancellationToken);
        }
        public async Task<Session?> GetSessionById(int id, CancellationToken cancellationToken)
        {
            return await _context.Sessions.Include(s => s.Movie).Include(s => s.Hall).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        //get sessions by movie id 
        //1 movie can have several sessions
        public async Task<ICollection<Session>> GetSessionsByMovieId(int movieId, CancellationToken cancellationToken)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Where(s => s.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }

        //get sessions by hallid
        //1 hall can have several sessions
        public async Task<ICollection<Session>> GetSessionsByHallId(int  hallId, CancellationToken cancellationToken)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Where(s => s.HallId == hallId)
                .ToListAsync(cancellationToken); 
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
