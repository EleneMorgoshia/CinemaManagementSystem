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
        private readonly IGenreRepository _genreRepository;
        public MovieService(IMovieRepository movieRepository, IUnitOfWork unitOfWork, IMapper mapper, IGenreRepository genreRepository)
        {
            _movieRepository = movieRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _genreRepository = genreRepository;
        }

        public async Task<ICollection<MovieDTO>> GetAllMoviesAsync(CancellationToken cancellationToken)
        {
            var movies = await _movieRepository.GetMovies(cancellationToken);
            return movies.Select(_mapper.Map<Movie, MovieDTO>).ToList();
        }

        public async Task<MovieDTO> GetMovieByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if (movie == null)
                throw new ArgumentException($"There is no movie with {id} id");
            return _mapper.Map<Movie, MovieDTO>(movie);
        }

        public async Task<MovieDTO> AddMovieAsync(CreateMovieDTO movieDto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(movieDto);
            await ValidateGenreIdsAsync(movieDto.GenreIds, cancellationToken);

            var movie = _mapper.Map<CreateMovieDTO, Movie>(movieDto);
            movie.MovieGenreRecords = movieDto.GenreIds.Select(id => new MovieGenre
            {
                GenreId = id
            }).ToList();

            await _movieRepository.AddMovie(movie, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<Movie, MovieDTO>(movie);
        }

        public async Task DeleteMovieAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if (movie == null)
                throw new ArgumentException($"There is no movie with {id} id");
            _movieRepository.DeleteMovie(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDto, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Movie ID must be greater than zero.");
            if (updateMovieDto == null)
                throw new ArgumentNullException(nameof(updateMovieDto));

            var movie = await _movieRepository.GetMovieById(id, cancellationToken);
            if (movie == null)
                throw new ArgumentException($"There is no movie with {id} id");

            if (updateMovieDto.GenreIds != null)
                await ValidateGenreIdsAsync(updateMovieDto.GenreIds, cancellationToken);
           
            _mapper.Map<UpdateMovieDTO, Movie>(updateMovieDto, movie);
            if (updateMovieDto.GenreIds != null)
                UpdateMovieGenres(movie, updateMovieDto.GenreIds);
            
            _movieRepository.UpdateMovie(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ValidateGenreIdsAsync(List<int> genreIds, CancellationToken cancellationToken)
        {
            if (genreIds == null || genreIds.Count == 0)
                throw new ArgumentException("Movie must have at least one genre.");

            if (genreIds.Any(id => id <= 0))
                throw new ArgumentException("Genre IDs must be positive.");

            if (genreIds.Distinct().Count() != genreIds.Count)
                throw new ArgumentException("Duplicate genre IDs are not allowed.");

            foreach (var genreId in genreIds)
            {
                var genre = await _genreRepository.GetGenreById(
                    genreId, cancellationToken);

                if (genre == null)
                    throw new KeyNotFoundException( $"Genre with ID {genreId} was not found.");
            }
        }

        //ადმინმა უნდა მომაწოდოს უკვე განახლებული ჟანრების სია
        //გადმოცემულ კონკრეტულ ფილმს მივამაგრებთ დააბდეითბულ ჟანრებს
        private void UpdateMovieGenres(Movie movie, List<int> genreIds)
        {
            //ვშლით ელემენტებს რომლებსაც არ მოიცავს genreIds
            var genresToRemove = movie.MovieGenreRecords
                .Where(mg => !genreIds.Contains(mg.GenreId))
                .ToList();

            
            foreach (var movieGenre in genresToRemove)
            {
                movie.MovieGenreRecords.Remove(movieGenre);
            }

  
            foreach (var genreId in genreIds)
            {
                // თუ ფილმს ეს ჟანრი ჯერ არ აქვს, ვამატებთ ახალ კავშირს.
                if (!movie.MovieGenreRecords.Any(mg => mg.GenreId == genreId))
                {
                    movie.MovieGenreRecords.Add(new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genreId
                    });
                }
            }
        }
    }
}


