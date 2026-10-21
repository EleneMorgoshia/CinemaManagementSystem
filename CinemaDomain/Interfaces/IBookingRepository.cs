using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IBookingRepository
    {
        Task<ICollection<Booking>> GetAllBookings(CancellationToken cancellationToken);
        Task<Booking?> GetBookingById(int id, CancellationToken cancellationToken);
        Task<ICollection<Booking>> GetBookingsByUserId(int userId, CancellationToken cancellationToken);
        Task<ICollection<Booking>> GetBookingsBySessionId(int sessionId, CancellationToken cancellationToken);
        Task<bool> IsSeatBookedForSession(int seatId, int sessionId, CancellationToken cancellationToken);
        Task AddBooking(Booking booking, CancellationToken cancellationToken);
        void DeleteBooking(Booking booking);
        void UpdateBooking(Booking booking);
    }
}
