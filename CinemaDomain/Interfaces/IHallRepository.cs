using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IHallRepository
    {
        Task<ICollection<Hall>> GetAll(CancellationToken cancellationToken);
        Task<Hall?> GetHallById(int id, CancellationToken cancellationToken);
        Task AddHall(Hall hall, CancellationToken cancellationToken);
        void DeleteHall(Hall hall);
        void UpdateHall(Hall hall);
    }
}
