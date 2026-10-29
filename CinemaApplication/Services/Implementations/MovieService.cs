using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Enums;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MovieService(IMovieRepository movieRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ICollection<MovieDTO>> GetAllMoviesAsync(CancellationToken cancellationToken)
        {
            var movies = await  _movieRepository.GetMovies(cancellationToken);
            return movies.Select(_mapper.Map<Movie, MovieDTO>).ToList();
        }

        public async Task<MovieDTO> GetMovieByIdAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if (movie == null)
                throw new ArgumentException($"There is no movie with {id} id");
            return _mapper.Map<Movie, MovieDTO>(movie);
        }

        public async Task<MovieDTO> AddMovieAsync(CreateMovieDTO movieDto, CancellationToken cancellationToken) 
        {
            if(movieDto == null)
                throw new ArgumentNullException(nameof(movieDto));

            var movie = _mapper.Map<CreateMovieDTO, Movie>(movieDto); 
            await _movieRepository.AddMovie(movie, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<Movie, MovieDTO>(movie);
        }

        public async Task DeleteMovieAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if(movie == null)
                throw new ArgumentException($"There is no movie with {id} id");
            _movieRepository.DeleteMovie(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateMovieAsync(int id , UpdateMovieDTO updateMovieDto, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            if(updateMovieDto == null)
                throw new ArgumentNullException(nameof(updateMovieDto));

            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if(movie == null)
                throw new ArgumentException($"There is no movie with {id} id");

            _mapper.Map<UpdateMovieDTO, Movie>(updateMovieDto, movie);
            _movieRepository.UpdateMovie(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}


