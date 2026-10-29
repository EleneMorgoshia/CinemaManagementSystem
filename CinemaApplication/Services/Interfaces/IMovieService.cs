using AutoMapper;
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
    public interface IMovieService
    {
        Task<ICollection<MovieDTO>> GetAllMoviesAsync(CancellationToken cancellationToken);
        Task<MovieDTO> GetMovieByIdAsync(int id, CancellationToken cancellationToken);
        Task<MovieDTO> AddMovieAsync(CreateMovieDTO movieDto, CancellationToken cancellationToken);
        Task DeleteMovieAsync(int id, CancellationToken cancellationToken);
        Task UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDto, CancellationToken cancellationToken);
    }
}
