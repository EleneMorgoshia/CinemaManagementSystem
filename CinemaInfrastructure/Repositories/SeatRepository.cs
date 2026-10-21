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
    public class SeatRepository : ISeatRepository
    {
        private readonly CinemaDbContext _context;
        public SeatRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Seat>> GetAllSeats(CancellationToken cancellationToken)
        {
            return await _context.Seats.ToListAsync(cancellationToken);
        }

        public async Task<Seat?> GetSeatById(int id, CancellationToken cancellationToken)
        {
            return await _context.Seats.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<ICollection<Seat>> GetSeatsByHallId(int hallId, CancellationToken cancellationToken)
        {
            return await _context.Seats.Where(s => s.HallId == hallId).ToListAsync(cancellationToken);
        }

        public async Task AddSeat(Seat seat, CancellationToken cancellationToken)
        {
            await _context.Seats.AddAsync(seat, cancellationToken);
        }

        public void Update(Seat seat)
        {
            _context.Seats.Update(seat);
        }

        public void Delete(Seat seat)
        {
            _context.Seats.Remove(seat);
        }
    }
}
