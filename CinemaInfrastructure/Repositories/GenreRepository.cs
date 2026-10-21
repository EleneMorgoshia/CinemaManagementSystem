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
    public class GenreRepository : IGenreRepository
    {
        private readonly CinemaDbContext _context;
        public GenreRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Genre>> GetAllGenres(CancellationToken cancellationToken)
        {
            return await _context.Genres.ToListAsync(cancellationToken);
        }

        public async Task<Genre?> GetGenreById(int id, CancellationToken cancellationToken)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Genre?> GetGenreByName(string genreName, CancellationToken cancellationToken)
        {
            return await _context.Genres.FirstOrDefaultAsync( g => g.GenreName == genreName, cancellationToken);
        }

        public async Task AddGenre(Genre genre, CancellationToken cancellationToken)
        {
            await _context.Genres.AddAsync(genre, cancellationToken);
        }

        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
        }

        public void Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
        }
    }
}
