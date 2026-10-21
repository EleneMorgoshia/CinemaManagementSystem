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
    public class TicketRepository : ITicketRepository
    {
        private readonly CinemaDbContext _context;
        public TicketRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Ticket>> GetAllTickets(CancellationToken cancellationToken)
        {
            return await _context.Tickets.ToListAsync(cancellationToken);
        }

        public async Task<Ticket?> GetTicketById(int id, CancellationToken cancellationToken)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<Ticket?> GetTicketByBookingId(int bookingId, CancellationToken cancellationToken)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.BookingId == bookingId, cancellationToken);
        }

        public async Task<Ticket?> GetTicketByCode(string ticketCode, CancellationToken cancellationToken)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.TicketCode == ticketCode, cancellationToken);
        }

        public async Task AddTicket(Ticket ticket, CancellationToken cancellationToken)
        {
            await _context.Tickets.AddAsync(ticket, cancellationToken);
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }

    }
}
