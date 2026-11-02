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
            //CreateMap<Movie, MovieDTO>();
            CreateMap<Movie, MovieDTO>()
            .ForMember(dest => dest.GenreIds,opt => opt.MapFrom(src =>
                src.MovieGenreRecords.Select(mg => mg.GenreId)));
            CreateMap<MovieDTO, Movie>();
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(dest => dest.MovieGenreRecords, opt => opt.Ignore());
            CreateMap<Movie, CreateMovieDTO>();
            CreateMap<Movie, UpdateMovieDTO>();
            CreateMap<UpdateMovieDTO, Movie>()
            .ForMember(dest => dest.MovieGenreRecords,
                opt => opt.Ignore());
            CreateMap<CreateMovieDTO, MovieDTO>();
        }
    }
}
