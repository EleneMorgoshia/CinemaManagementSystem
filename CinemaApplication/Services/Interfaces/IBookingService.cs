using CinemaApplication.Services.Implementations;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ICollection<BookingDTO>> GetAllBookingsAsync(CancellationToken cancellationToken);
        Task<BookingDTO> GetBookingByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken);
        Task<ICollection<BookingDTO>> GetBookingsByUserIdAsync(string userId, CancellationToken cancellationToken);

        Task<ICollection<BookingDTO>> GetBookingsBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
        Task<BookingDTO> AddBookingAsync(CreateBookingDTO creatingBookingDTO, string userId, CancellationToken cancellationToken);
        Task UpdateBookingAsync(int bookingId, CancelBookingDTO cancelBookingDTO, string userId, CancellationToken cancellationToken);
        Task DeleteBookingAsync(int bookingId, CancellationToken cancellationToken);

    }
}
