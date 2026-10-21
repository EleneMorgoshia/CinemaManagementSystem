using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface ISeatRepository
    {
        Task<ICollection<Seat>> GetAllSeats(CancellationToken cancellationToken);
        Task<Seat?> GetSeatById(int id, CancellationToken cancellationToken);
        Task<ICollection<Seat>> GetSeatsByHallId(int hallId, CancellationToken cancellationToken);
        Task AddSeat(Seat seat, CancellationToken cancellationToken);
        void Update(Seat seat);
        void Delete(Seat seat);
    }
}
 