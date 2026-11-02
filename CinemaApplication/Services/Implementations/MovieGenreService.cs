using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class MovieGenreService : IMovieGenreService
    {
        private readonly IMovieGenreRepository _movieGenreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        public MovieGenreService(IMovieGenreRepository movieGenreRepository,  IUnitOfWork unitOfWork,IMovieRepository movieRepository, IGenreRepository genreRepository,  IMapper mapper)
        {
            _movieGenreRepository = movieGenreRepository;
            _unitOfWork = unitOfWork;
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        public async Task<ICollection<MovieGenreDTO>> GetAllMovieGenresAsync(CancellationToken cancellationToken)
        {
            var movieGenres = await _movieGenreRepository.GetAllMovieGenresAsync(cancellationToken);

            return movieGenres.Select(_mapper.Map<MovieGenreDTO>).ToList();
        }
        public async Task<MovieGenreDTO> AddGenreToMovieAsync(int movieId,int genreId,CancellationToken cancellationToken)
        {
            if (movieId <= 0 || genreId <= 0)
                throw new ArgumentException("Invalid movie or genre ID.");

            //reached movie
            var movie = await _movieRepository.GetMovieById(movieId, cancellationToken);
            if (movie == null)
                throw new KeyNotFoundException("Movie not found.");

            //reeached genre
            var genre = await _genreRepository.GetGenreById(genreId, cancellationToken);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            var existingMovieGenre = await _movieGenreRepository.GetMovieGenre(movieId, genreId, cancellationToken);
            if (existingMovieGenre != null)
                throw new InvalidOperationException("This genre is already assigned to the movie.");

            //creating object
            var movieGenre = new MovieGenre
            {
                MovieId = movieId,
                GenreId = genreId
            };

            await _movieGenreRepository.AddMovieGenreAsync(movieGenre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<MovieGenreDTO>(movieGenre);
        }

        public async Task<ICollection<GenreDTO>> GetGenresByMovieIdAsync(int movieId,CancellationToken cancellationToken)
        {
            if (movieId <= 0)
                throw new ArgumentOutOfRangeException(nameof(movieId));

            var movie = await _movieRepository.GetMovieById(movieId, cancellationToken);
            if (movie == null)
                throw new KeyNotFoundException("Movie not found.");

            var movieGenres = await _movieGenreRepository .GetGenresByMovieIdAsync(movieId, cancellationToken);

            return movieGenres.Select(mg => _mapper.Map<GenreDTO>(mg.Genre)).ToList();
        }

        public async Task<ICollection<MovieDTO>> GetMoviesByGenreIdAsync(int genreId, CancellationToken cancellationToken)
        {
            if (genreId <= 0)
                throw new ArgumentOutOfRangeException(nameof(genreId));

            var genre = await _genreRepository.GetGenreById(genreId, cancellationToken);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            var movieGenres = await _movieGenreRepository.GetMoviesByGenreIdAsync(genreId, cancellationToken);

            return movieGenres.Select(mg => _mapper.Map<MovieDTO>(mg.Movie)).ToList();
        }
        public async Task RemoveGenreFromMovieAsync(int movieId, int genreId, CancellationToken cancellationToken)
        {
            if (movieId <= 0 || genreId <= 0)
                throw new ArgumentException("Invalid movie or genre ID.");

            var movieGenre = await _movieGenreRepository.GetMovieGenre(movieId, genreId, cancellationToken);
            if (movieGenre == null)
                throw new KeyNotFoundException("This genre is not assigned to the movie.");

            //ერთი ჟანრი მინიმუმ მაინც უნდა ქონდეს ფილმს
            var movieGenres = await _movieGenreRepository.GetGenresByMovieIdAsync(movieId, cancellationToken);
            if (movieGenres.Count <= 1)
                throw new InvalidOperationException("A movie must have at least one genre.");
            
            _movieGenreRepository.DeleteMovieGenre(movieGenre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}
