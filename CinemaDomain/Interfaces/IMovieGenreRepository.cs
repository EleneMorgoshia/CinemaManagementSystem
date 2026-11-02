using CinemaDomain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IMovieGenreRepository
    {
        Task<ICollection<MovieGenre>> GetAllMovieGenresAsync(CancellationToken cancellationToken);
        Task<MovieGenre?> GetMovieGenre(int movieId, int genreId, CancellationToken cancellationToken);
        Task AddMovieGenreAsync(MovieGenre movieGenre, CancellationToken cancellationToken);
        Task<ICollection<MovieGenre>> GetGenresByMovieIdAsync(int movieId, CancellationToken cancellationToken);
        Task<ICollection<MovieGenre>> GetMoviesByGenreIdAsync(int genreId, CancellationToken cancellationToken);
        void DeleteMovieGenre(MovieGenre movieGenre);

    }
}
