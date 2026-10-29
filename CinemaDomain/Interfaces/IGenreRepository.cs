using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IGenreRepository
    {
        Task<ICollection<Genre>> GetAllGenres(CancellationToken cancellationToken);
        Task<Genre?> GetGenreById(int id, CancellationToken cancellationToken);
        //Task<Genre?> GetGenreByName(string genreName, CancellationToken cancellationToken);
        Task<ICollection<Genre>> GetGenresByMovieName(string movieName, CancellationToken cancellationToken);
        Task AddGenre(Genre genre, CancellationToken cancellationToken);
        void Update(Genre genre);
        void Delete(Genre genre);
    }
} 
