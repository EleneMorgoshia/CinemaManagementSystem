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
        Task<MovieGenre?> GetMovieGenre(int movieId, int genreId, CancellationToken cancellationToken);
        Task AddMovieGenreAsync(MovieGenre movieGenre, CancellationToken cancellationToken);
        public void DeleteMovieGenre(MovieGenre movieGenre);

    }
}
