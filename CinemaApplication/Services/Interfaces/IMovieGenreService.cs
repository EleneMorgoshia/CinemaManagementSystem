using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Interfaces
{
    public interface IMovieGenreService
    {
        Task<ICollection<MovieGenreDTO>> GetAllMovieGenresAsync(CancellationToken cancellationToken);
        Task<MovieGenreDTO> AddGenreToMovieAsync(int movieId, int genreId, CancellationToken cancellationToken);
        Task<ICollection<GenreDTO>> GetGenresByMovieIdAsync(int movieId, CancellationToken cancellationToken);
        Task<ICollection<MovieDTO>> GetMoviesByGenreIdAsync(int genreId, CancellationToken cancellationToken);
        Task RemoveGenreFromMovieAsync(int movieId, int genreId, CancellationToken cancellationToken);
    }
}
