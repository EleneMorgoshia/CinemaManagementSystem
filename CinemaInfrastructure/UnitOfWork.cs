using CinemaDomain.Interfaces;
using CinemaInfrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaInfrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CinemaDbContext _context;
        public UnitOfWork(CinemaDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
