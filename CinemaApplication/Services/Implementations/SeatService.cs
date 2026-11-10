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
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SeatService(ISeatRepository seatRepository, IUnitOfWork unitOfWork, 
            IMapper mapper, IHallRepository hallRepository, 
            IBookingRepository bookingRepository, ISessionRepository sessionRepository)
        {
            _seatRepository = seatRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hallRepository = hallRepository;
            _bookingRepository = bookingRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<ICollection<SeatDTO>> GetAllSeatsAsync(CancellationToken cancellationToken)
        {
            var seats = await _seatRepository.GetAllSeats(cancellationToken);
            return seats.Select(_mapper.Map<Seat, SeatDTO>).ToList();              
        }
        
        public async Task<SeatDTO> GetSeatByIdAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Seat ID must be greater than zero.");
            
            var seat = await _seatRepository.GetSeatById(id, cancellationToken);
            if(seat == null)
                throw new KeyNotFoundException($"Seat with ID {id} was not found.");

            return _mapper.Map<Seat, SeatDTO>(seat);
        }

        public async Task<ICollection<SeatDTO>> GetSeatsByHallIdAsync(int hallId, CancellationToken cancellationToken)
        {
            if(hallId <= 0)
                throw new ArgumentOutOfRangeException(nameof(hallId), "Hall ID must be greater than zero.");

            var hall = await _hallRepository.GetHallById(hallId, cancellationToken);
            if(hall == null)
                throw new KeyNotFoundException($"Hall with ID {hallId} was not found.");
                
            var seats = await _seatRepository.GetSeatsByHallId(hallId, cancellationToken);
            return seats.Select(_mapper.Map<Seat, SeatDTO>).ToList();
        }

        public async Task<SeatDTO> AddSeatAsync(CreateSeatDTO createSeatDTO, CancellationToken cancellationToken)
        {
            if(createSeatDTO == null) 
                throw new ArgumentNullException(nameof(createSeatDTO));

            //hall check
            var hall = await _hallRepository.GetHallById(createSeatDTO.HallId, cancellationToken);
            if(hall == null)
                throw new KeyNotFoundException($"Hall with ID {createSeatDTO.HallId} was not found.");
            
            //existing seats in hall
            var existingSeats = await _seatRepository.GetSeatsByHallId(createSeatDTO.HallId, cancellationToken);
            bool seatExists = existingSeats.Any(s => s.RowNumber == createSeatDTO.RowNumber && s.SeatNumber == createSeatDTO.SeatNumber);
            if (seatExists)
                throw new InvalidOperationException("A seat with this row and number already exists in this hall.");

            var seat = _mapper.Map<CreateSeatDTO, Seat>(createSeatDTO);
            await _seatRepository.AddSeat(seat, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<Seat, SeatDTO>(seat);
        }

        public async Task UpdateSeatAsync(int id, UpdateSeatDTO updateSeatDTO, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Seat ID must be greater than zero.");
            ArgumentNullException.ThrowIfNull(updateSeatDTO);

            var seat = await _seatRepository.GetSeatById(id, cancellationToken);
            if (seat == null)
                throw new KeyNotFoundException($"Seat with ID {id} was not found.");

            _mapper.Map(updateSeatDTO, seat);
            _seatRepository.Update(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSeatAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Seat ID must be greater than zero.");

            var seat = await _seatRepository.GetSeatById(id, cancellationToken);
            if(seat == null)
                throw new KeyNotFoundException($"Seat with ID {id} was not found.");

            //booking check
            var bookings = await _bookingRepository.GetAllBookings(cancellationToken);
            if(bookings.Any(b => b.SeatId == id))
                throw new InvalidOperationException("Cannot delete a seat that has existing bookings.");

            _seatRepository.Delete(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<ICollection<SeatDTO>> GetAvaliableSeatsBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetSessionById(sessionId, cancellationToken);
            if(session == null)
                throw new KeyNotFoundException($"Session with ID {sessionId} was not found.");

            //ყველა სითი რომელიც ამ სესიის კონკრეტულ ჰოლში გვაქვს 
            var seats = await _seatRepository.GetSeatsByHallId(session.HallId, cancellationToken);
            
            //BOOKING ები ამ კონკრეტული სესიის
            var bookings = await _bookingRepository.GetBookingsBySessionId(sessionId, cancellationToken);

            var bookedSeatsIds = bookings
                .Where(booking => booking.Status)
                .Select(booking => booking.SeatId)
                .ToHashSet();
            
            var availableSeats = seats
                .Where(seat => !bookedSeatsIds.Contains(seat.Id))
                .Select(seat => _mapper.Map<SeatDTO>(seat))
                .ToList();
            
            return availableSeats;
        }
    }
}
