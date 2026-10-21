using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface ITicketRepository
    {
        Task<ICollection<Ticket>> GetAllTickets(CancellationToken cancellationToken);
        Task<Ticket?> GetTicketById(int id, CancellationToken cancellationToken);
        Task<Ticket?> GetTicketByBookingId(int bookingId, CancellationToken cancellationToken);
        Task<Ticket?> GetTicketByCode(string ticketCode, CancellationToken cancellationToken);
        Task AddTicket(Ticket ticket, CancellationToken cancellationToken);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
    }
}
