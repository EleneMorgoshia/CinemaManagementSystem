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
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionRepository _sessionRpeository;
        private readonly IMapper _mapper;
        public HallService(IHallRepository hallRepository, IUnitOfWork unitOfWork, IMapper mapper, ISessionRepository sessionRpeository)
        {
            _hallRepository = hallRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sessionRpeository = sessionRpeository;
        }

        public async Task<ICollection<HallDTO>> GetAllHallsAsync(CancellationToken cancellationToken)
        {
            var halls = await _hallRepository.GetAll(cancellationToken);
            return halls.Select(_mapper.Map<Hall, HallDTO>).ToList();
        }

        public async Task<HallDTO> GetHallByIdAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Hall ID must be greater than zero.");

            var movie = await _hallRepository.GetHallById(id, cancellationToken);
            if(movie == null)
                throw new ArgumentException($"There is no hall with {id} id");
            return _mapper.Map<Hall, HallDTO>(movie);
        }

        public async Task<HallDTO> AddHallAsync(CreateHallDTO createHallDTO, CancellationToken cancellationToken)
        {
            if(createHallDTO == null)
                throw new ArgumentNullException(nameof(createHallDTO));
            
            var hall = _mapper.Map<CreateHallDTO, Hall>(createHallDTO);
            
            await _hallRepository.AddHall(hall, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CreateHallDTO, HallDTO>(createHallDTO);
        }

        public async Task UpdateHallAsync(int id, UpdateHallDTO updateHallDTO, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Hall ID must be greater than zero.");

            ArgumentNullException.ThrowIfNull(updateHallDTO);
            var hall = await _hallRepository.GetHallById(id, cancellationToken);
            if(hall == null)
                throw new ArgumentException($"There is no hall with {id} id");

            _mapper.Map<UpdateHallDTO, Hall>(updateHallDTO,hall);
            _hallRepository.UpdateHall(hall);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteHallAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Hall ID must be greater than zero.");
            
            var hall = await _hallRepository.GetHallById(id, cancellationToken);
            if (hall == null)
                throw new KeyNotFoundException($"Hall with ID {id} was not found.");

            var sessions = await _sessionRpeository.GetSessionsByHallId(id, cancellationToken);
            if(sessions.Any())
                throw new InvalidOperationException("Cannot delete a hall that has sessions.");
            
            _hallRepository.DeleteHall(hall);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
