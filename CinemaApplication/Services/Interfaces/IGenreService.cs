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
    public interface IGenreService
    {
        Task<ICollection<GenreDTO>> GetAllGenresAsync(CancellationToken cancellationToken);
        Task<GenreDTO> GetGenreByIdAsync(int id, CancellationToken cancellationToken);
        Task<GenreDTO> AddGenreAsync(CreateGenreDTO createGenreDTO, CancellationToken cancellationToken);
        Task UpdateGenreAsync(int id, UpdateGenreDTO updateGenreDTO, CancellationToken cancellationToken);
        Task DeleteGenreAsync(int id, CancellationToken cancellationToken);
        Task<ICollection<GenreDTO>> GetGenresByMovieNameAsync(string movieName, CancellationToken cancellationToken);

    }
}
