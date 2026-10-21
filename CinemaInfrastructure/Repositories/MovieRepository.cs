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
    public class MovieRepository : IMovieRepository
    {
        private readonly CinemaDbContext _context;
        public MovieRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Movie>> GetMovies(CancellationToken cancelationToken)
        {
            return await _context.Movies.ToListAsync(cancelationToken);
        }

        public async Task<Movie?> GetMovieById(int id, CancellationToken cancelationToken)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id, cancelationToken);
        }

        public async Task AddMovie(Movie movie, CancellationToken cancelationToken)
        {
            await _context.Movies.AddAsync(movie, cancelationToken);
        }

        //ეს ორი მეთოდი არის სინქრონული.(unitofwork იზავს მერე სერვისიდან სინქრონიზაციას)
        public void DeleteMovie(Movie movie)
        {
             _context.Movies.Remove(movie);
        }

        public void UpdateMovie(Movie movie)
        {
            _context.Movies.Update(movie);
        }
    }
}
