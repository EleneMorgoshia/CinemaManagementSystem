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
    public interface  ITicketService
    {
        Task<ICollection<TicketDTO>> GetAllTicketsAsync(CancellationToken cancellationToken);
        Task<TicketDTO> GetTicketByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken);
        Task<TicketDTO> GetTicketByCodeAsync(string ticketCode, string userId, bool isAdmin, CancellationToken cancellationToken);
        Task<TicketDTO> GetTicketByBookingIdAsync(int bookingId, string userId, bool isAdmin, CancellationToken cancellationToken);
        Task AddTicketAsync(Booking booking, decimal finalPrice, CancellationToken cancellationToken);
    }
}
