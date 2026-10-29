using AutoMapper;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class MovieGenreService : IMovieGenreService
    {
        private readonly IMovieGenreRepository _movieGenreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public MovieGenreService(IMovieGenreRepository movieGenreRepository,  IUnitOfWork unitOfWork,  IMapper mapper)
        {
            _movieGenreRepository = movieGenreRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
    }
}
