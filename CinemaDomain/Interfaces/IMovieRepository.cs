using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IMovieRepository
    {
        Task<ICollection<Movie>> GetMovies(CancellationToken cancelationToken);
        Task<Movie?> GetMovieById(int id, CancellationToken cancelationToken);
        Task AddMovie(Movie movie, CancellationToken cancelationToken);
        void DeleteMovie(Movie movie);
        void UpdateMovie(Movie movie);
    
    }
}
 