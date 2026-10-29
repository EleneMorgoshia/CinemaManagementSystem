using AutoMapper;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieDTO>();
            CreateMap<MovieDTO, Movie>();
            CreateMap<CreateMovieDTO, Movie>();
            CreateMap<Movie, CreateMovieDTO>();
            CreateMap<Movie, UpdateMovieDTO>();
            CreateMap<UpdateMovieDTO, Movie>();
            CreateMap<CreateMovieDTO, MovieDTO>();
        }
    }
}
