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
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionService(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, IMapper mapper, IHallRepository hallRepository, IMovieRepository movieRepository)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hallRepository = hallRepository;
            _movieRepository = movieRepository;
        }

        public async Task<ICollection<SessionDTO>> GetSessionsAsync(CancellationToken cancellationToken)
        {
            var sessions = await _sessionRepository.GetSessions(cancellationToken);
            return sessions.Select(_mapper.Map<Session, SessionDTO>).ToList();
        }

        public async Task<SessionDTO> GetSessionByIdAsync(int id, CancellationToken cancellationToken) 
        {
            if (id <= 0)
                throw new ArgumentException("Id must be grater than 0");
            var session = await _sessionRepository.GetSessionById(id, cancellationToken);
            if(session == null)
                throw new KeyNotFoundException($"Session with ID {id} was not found.");
            return _mapper.Map<Session, SessionDTO>(session);
        }

        public async Task<ICollection<SessionDTO>> GetSessionsByMovieIdAsync(int movieId, CancellationToken cancellationToken)
        {
            if(movieId <= 0)
                throw new ArgumentOutOfRangeException(nameof(movieId));

            var movie = await _movieRepository.GetMovieById(movieId, cancellationToken);
            if(movie == null)
                throw new KeyNotFoundException($"Movie with ID {movieId} was not found.");
            
            var sessions = await _sessionRepository.GetSessionsByMovieId(movieId, cancellationToken);
            return sessions.Select(_mapper.Map<Session, SessionDTO>).ToList();
        }

        public async Task<ICollection<SessionDTO>> GetSessionsByHallIdAsync(int hallId, CancellationToken cancellationToken)
        {
            if (hallId <= 0)
                throw new ArgumentOutOfRangeException(nameof(hallId));

            var hall = await _hallRepository.GetHallById(hallId, cancellationToken);
            if(hall == null)
                throw new KeyNotFoundException($"Hall with ID {hallId} was not found.");

            var sessions = await _sessionRepository.GetSessionsByHallId(hallId, cancellationToken);
            return sessions.Select(_mapper.Map<Session, SessionDTO>).ToList();
        }

        public async Task<SessionDTO> AddSessionAsync(CreateSessionDTO createSessionDto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(createSessionDto);
            var session = _mapper.Map<CreateSessionDTO, Session>(createSessionDto);
            await ValidateSessionAsync(session, cancellationToken);

            await _sessionRepository.AddSession(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<Session, SessionDTO>(session);
        }

        
        private async Task ValidateSessionAsync(Session session,CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(session);

            //movie check
            if (session.MovieId <= 0)
                throw new ArgumentOutOfRangeException(nameof(session.MovieId));

            var movie = await _movieRepository.GetMovieById(session.MovieId, cancellationToken);
            if (movie == null)
                throw new KeyNotFoundException( $"Movie with ID {session.MovieId} was not found.");

            //hall check 
            if (session.HallId <= 0)
                throw new ArgumentOutOfRangeException(nameof(session.HallId));

            var hall = await _hallRepository.GetHallById(session.HallId, cancellationToken);
            if (hall == null)
                throw new KeyNotFoundException( $"Hall with ID {session.HallId} was not found.");

            //starting time
            if (session.StartsAt <= DateTime.Now)
                throw new ArgumentException( "Session must start in the future.");

            //prices
            if (session.StandardPrice < 0 || session.VipPrice < 0)
                throw new ArgumentException("Session prices cannot be negative.");

            //calculate session end time
            var newSessionEnd = session.StartsAt.AddMinutes(movie.DurationMinutes);

            //get existing sessions in the same hall
            var existingSessions = await _sessionRepository.GetSessionsByHallId(session.HallId, cancellationToken);

            foreach (var existingSession in existingSessions)
            {
                var existingSessionEnd = existingSession.StartsAt.AddMinutes(existingSession.Movie.DurationMinutes);

                // Check session overlap
                if (session.StartsAt < existingSessionEnd && existingSession.StartsAt < newSessionEnd)
                    throw new InvalidOperationException("This session overlaps with another session in the same hall.");
            }
        }
    }
}
