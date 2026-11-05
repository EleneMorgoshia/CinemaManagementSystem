using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        //private readonly IUsermManagerService _userManagerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketService _ticketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BookingService(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ISeatRepository seatRepository, ISessionRepository sessionRepository, ITicketService ticketService)
        {
            _bookingRepository = bookingRepository;
            //_userManagerService = usermManagerService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _seatRepository = seatRepository;
            _sessionRepository = sessionRepository;
            _ticketService = ticketService;
        }

        public async Task<ICollection<BookingDTO>> GetAllBookingsAsync(CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookings(cancellationToken);
            return bookings.Select(_mapper.Map<Booking, BookingDTO>).ToList();
        }
        
        public async Task<BookingDTO> GetBookingByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Booking ID must be greater than zero.");
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var booking = await _bookingRepository.GetBookingById(id, cancellationToken);
            if(booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} was not found.");

            if (booking.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("You cannot access another user's booking.");
            return _mapper.Map<Booking,BookingDTO>(booking);
        }

        public async Task<ICollection<BookingDTO>> GetBookingsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} was not found.");

            var bookings = await _bookingRepository.GetBookingsByUserId(userId, cancellationToken);
            return bookings.Select(_mapper.Map<Booking, BookingDTO>).ToList();
        }

        public async Task<ICollection<BookingDTO>> GetBookingsBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
        {
            if(sessionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "Session ID must be greater than zero.");
            
            var session = await _sessionRepository.GetSessionById(sessionId, cancellationToken);
            if(session == null)
                throw new KeyNotFoundException($"Session with ID {sessionId} was not found.");

            var bookings = await _bookingRepository.GetBookingsBySessionId(sessionId, cancellationToken);
            return bookings.Select(_mapper.Map<Booking, BookingDTO>).ToList();
        }

        public async Task<BookingDTO> AddBookingAsync(CreateBookingDTO creatingBookingDTO, string userId, CancellationToken cancellationToken)
        {

            ArgumentNullException.ThrowIfNull(creatingBookingDTO);
            await ValidateBookingAsync(creatingBookingDTO, userId, cancellationToken);

            //session - standard price & vip price
            var session = await _sessionRepository.GetSessionById(creatingBookingDTO.SessionId, cancellationToken);
            var seat = await _seatRepository.GetSeatById(creatingBookingDTO.SeatId, cancellationToken);
            if (session == null || seat == null)
                throw new InvalidOperationException("Session or seat not found.");

            decimal finalPrice = seat.IsVip ? session.VipPrice : session.StandardPrice;

            var booking = _mapper.Map<CreateBookingDTO, Booking>(creatingBookingDTO);
            booking.UserId = userId;
            booking.Status = true;
            booking.CreatedAt = DateTime.UtcNow;

            await _bookingRepository.AddBooking(booking, cancellationToken);
            await _ticketService.AddTicketAsync(booking, finalPrice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task UpdateBookingAsync(int bookingId, CancelBookingDTO cancelBookingDTO, string userId, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(cancelBookingDTO);
            if(bookingId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bookingId));

            if (bookingId != cancelBookingDTO.Id)
                throw new ArgumentException("Booking IDs do not match.");


            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var booking = await _bookingRepository.GetBookingById(bookingId, cancellationToken);
            if( booking == null )
                throw new KeyNotFoundException("Booking not found.");
          
            var user = await _userManager.FindByIdAsync(userId);
            if ( user == null )
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You cannot cancel another user's booking.");
            if (!booking.Status)
                throw new InvalidOperationException("Booking is already cancelled.");

            //Session cehck
            var session = await _sessionRepository.GetSessionById(booking.SessionId, cancellationToken);

            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.StartsAt <= DateTime.UtcNow)
                throw new InvalidOperationException("You cannot cancel a booking after the session has started.");

            booking.Status = false;

            _bookingRepository.UpdateBooking(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBookingAsync(int bookingId, CancellationToken cancellationToken)
        {
            if (bookingId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bookingId));

            var booking = await _bookingRepository.GetBookingById(bookingId, cancellationToken);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            _bookingRepository.DeleteBooking(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        private async Task ValidateBookingAsync(CreateBookingDTO dto, string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException( $"User with ID {userId} was not found.");
            
            var session = await _sessionRepository.GetSessionById(dto.SessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.StartsAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Session has already started.");
   
            if (!session.Status)
                throw new InvalidOperationException(
                    "This session is not available for booking.");

            var seat = await _seatRepository.GetSeatById(
                dto.SeatId, cancellationToken);

            if (seat == null)
                throw new KeyNotFoundException("Seat not found.");

            if (seat.HallId != session.HallId)
                throw new InvalidOperationException("Seat does not belong to the session's hall.");

            var isBooked = await _bookingRepository.IsSeatBookedForSession(dto.SeatId, dto.SessionId, cancellationToken);

            if (isBooked)
                throw new InvalidOperationException("This seat is already booked.");
        }
    }
}
