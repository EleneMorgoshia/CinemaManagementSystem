using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public  class TicketService : ITicketService 
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<ICollection<TicketDTO>> GetAllTicketsAsync(CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetAllTickets(cancellationToken);
            return tickets.Select(_mapper.Map<Ticket, TicketDTO>).ToList();
        }

        public async Task<TicketDTO> GetTicketByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Ticket ID must be greater than zero.");
            
            var ticket = await _ticketRepository.GetTicketById(id, cancellationToken);
            if(ticket == null)
                throw new ArgumentException($"There is no ticket with {id} id");

            await ValidateTicketAccessAsync(ticket, userId, isAdmin, cancellationToken);

            return _mapper.Map<Ticket, TicketDTO>(ticket);
        }

        public async Task<TicketDTO> GetTicketByCodeAsync(string ticketCode, string userId, bool isAdmin, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ticketCode))
                throw new ArgumentException("Ticket code cannot be empty");

            var ticket = await _ticketRepository.GetTicketByCode(ticketCode, cancellationToken);
            if( ticket == null )
                throw new ArgumentException($"There is no ticket with {ticketCode} ticket code");

            await ValidateTicketAccessAsync(ticket, userId, isAdmin, cancellationToken);
            return _mapper.Map<Ticket, TicketDTO>(ticket);
        }

        public async Task<TicketDTO> GetTicketByBookingIdAsync(int bookingId, string userId, bool isAdmin, CancellationToken cancellationToken)
        {
            if (bookingId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bookingId), "Ticket ID must be greater than zero.");

            var ticket = await _ticketRepository.GetTicketByBookingId(bookingId, cancellationToken);
            if(ticket == null)
                throw new ArgumentException($"There is no ticket with {bookingId} booking id");
            await ValidateTicketAccessAsync(ticket, userId, isAdmin, cancellationToken);
            return _mapper.Map<Ticket, TicketDTO>(ticket);
        }
        public async Task AddTicketAsync(Booking booking, decimal finalPrice,  CancellationToken cancellationToken)
        {
            await ValidateTicketAsync(booking, cancellationToken);
            if (finalPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(finalPrice));

            var ticket = new Ticket
            {
                Booking = booking,
                TicketCode = Guid.NewGuid().ToString("N"), //Guid.NewGuid() - ავტომატურად ქმნის რენდომ უნიკალურ ინდენთიფიკატორს. TოString ფარდაქმნსი სტრინგად 
                FinalPrice = finalPrice
            };

            await _ticketRepository.AddTicket(ticket, cancellationToken);
        }

        private async Task ValidateTicketAsync(Booking booking, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(booking);
            if (booking.Ticket != null)
                throw new InvalidOperationException("This booking already has a ticket.");
            
            if (!booking.Status)
                throw new InvalidOperationException("Cannot create a ticket for a cancelled booking.");

            if (booking.Id > 0)
            {
                var existingBooking = await _bookingRepository.GetBookingById(booking.Id, cancellationToken);
                if (existingBooking == null)
                    throw new KeyNotFoundException("Booking not found.");

                var existingTicket = await _ticketRepository.GetTicketByBookingId(booking.Id, cancellationToken);
                if (existingTicket != null)
                    throw new InvalidOperationException("This booking already has a ticket.");
            }
        }

        private async Task ValidateTicketAccessAsync(Ticket ticket,string userId, bool isAdmin, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var booking = await _bookingRepository.GetBookingById(
                ticket.BookingId, cancellationToken);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            if (booking.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException(
                    "You cannot access another user's ticket.");
        }

    }
}
