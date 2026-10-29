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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GenreService(IGenreRepository genreRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ICollection<GenreDTO>> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreRepository.GetAllGenres(cancellationToken);
            return genres.Select(_mapper.Map<Genre, GenreDTO>).ToList();
        }

        public async Task<GenreDTO> GetGenreByIdAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException("id");
            var genre = await _genreRepository.GetGenreById(id, cancellationToken);
            if (genre == null)
                throw new ArgumentException($"There is no genre with {id} id");
            return _mapper.Map<Genre, GenreDTO>(genre);
        }

        public async Task<GenreDTO> AddGenreAsync(CreateGenreDTO createGenreDTO, CancellationToken cancellationToken)
        {
            if (createGenreDTO == null)
                throw new ArgumentNullException(nameof(createGenreDTO));

            var genre = _mapper.Map<CreateGenreDTO, Genre>(createGenreDTO);
            await _genreRepository.AddGenre(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<Genre, GenreDTO>(genre);
        }

        public async Task UpdateGenreAsync(int id, UpdateGenreDTO updateGenreDTO, CancellationToken cancellationToken)
        {
            if(id <= 0) 
                throw new ArgumentOutOfRangeException("id");
            if(updateGenreDTO == null)
                throw new ArgumentNullException(nameof(updateGenreDTO));

            var genre = await _genreRepository.GetGenreById(id, cancellationToken);
            if (genre == null)
                throw new ArgumentException($"There is no genre with {id} id");
            _mapper.Map<UpdateGenreDTO, Genre>(updateGenreDTO, genre);
            _genreRepository.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGenreAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) 
                throw new ArgumentOutOfRangeException("id");
            var genre = await _genreRepository.GetGenreById(id, cancellationToken);
            if(genre == null)
                throw new ArgumentException($"There is no genre with {id} id");
            _genreRepository.Delete(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<ICollection<GenreDTO>> GetGenresByMovieNameAsync(string movieName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieName))
                throw new ArgumentException("Movie name cannot be empty");
            var genres = await _genreRepository.GetGenresByMovieName(movieName.Trim(), cancellationToken);
            return genres.Select(_mapper.Map<Genre, GenreDTO>).ToList();
        }

    }
}
