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
    public class HallRepository : IHallRepository
    {
        private readonly CinemaDbContext _context;
        public HallRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Hall>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Halls.ToListAsync(cancellationToken);
        }

        public async Task<Hall?> GetHallById(int id, CancellationToken cancellationToken)
        {
            return await _context.Halls.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task AddHall(Hall hall, CancellationToken cancellationToken)
        {
            await _context.Halls.AddAsync(hall, cancellationToken);
        }

        public void DeleteHall(Hall hall)
        {
            _context.Halls.Remove(hall);
        }

        public void UpdateHall(Hall hall) 
        {
            _context.Halls.Update(hall);
        }
    }
}
