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
    public class BookingRepository : IBookingRepository
    {
        private readonly CinemaDbContext _context;
        public BookingRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Booking>> GetAllBookings(CancellationToken cancellationToken)
        {
            return await _context.Bookings.ToListAsync(cancellationToken);
        }

        public async Task<Booking?> GetBookingById(int id, CancellationToken cancellationToken)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<ICollection<Booking>> GetBookingsByUserId(int userId, CancellationToken cancellationToken)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Booking>> GetBookingsBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            return await _context.Bookings.Where(b => b.SessionId == sessionId).ToListAsync(cancellationToken);
        }

        //კონკრეტული სესიისთვის კონკრეტული ადგილი არის თუ არა დაკავებული
        public async Task<bool> IsSeatBookedForSession(int seatId, int sessionId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .AnyAsync(b => b.SeatId == seatId && b.SessionId == sessionId && b.Status == true, cancellationToken);
        }

        public async Task AddBooking(Booking booking, CancellationToken cancellationToken) 
        { 
            await _context.Bookings.AddAsync(booking, cancellationToken);
        }

        public void DeleteBooking(Booking booking) 
        { 
            _context.Bookings.Remove(booking);
        } 

        public void UpdateBooking(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

    }
}
