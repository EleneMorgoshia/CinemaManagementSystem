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
    public class MovieGenreRepository : IMovieGenreRepository
    {
        private readonly CinemaDbContext _context;
        public MovieGenreRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<MovieGenre>> GetAllMovieGenresAsync(CancellationToken cancellationToken)
        {
            return await _context.MovieGenres.ToListAsync(cancellationToken);
        }
        public async Task<MovieGenre?> GetMovieGenre(int movieId, int genreId, CancellationToken cancellationToken)
        {
            return await _context.MovieGenres.FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId, cancellationToken);
        }

        public async Task AddMovieGenreAsync(MovieGenre movieGenre, CancellationToken cancellationToken)
        { 
            await _context.MovieGenres.AddAsync(movieGenre, cancellationToken);
        }
        public async Task<ICollection<MovieGenre>> GetGenresByMovieIdAsync(int movieId, CancellationToken cancellationToken)
        {
            return await _context.MovieGenres.Include(mg => mg.Genre)
                .Where(mg => mg.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<MovieGenre>> GetMoviesByGenreIdAsync(int genreId,CancellationToken cancellationToken)
        {
            return await _context.MovieGenres.Include(mg => mg.Movie)
                .Where(mg => mg.GenreId == genreId)
                .ToListAsync(cancellationToken);
        }
        public void DeleteMovieGenre(MovieGenre movieGenre)
        {
            _context.MovieGenres.Remove(movieGenre);
        }

    }
}
