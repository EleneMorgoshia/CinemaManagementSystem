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
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreDTO, Genre>();

            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<Genre, CreateGenreDTO>();

            CreateMap<UpdateGenreDTO, Genre>();
            CreateMap<Genre, UpdateGenreDTO>();
        }
    }
}
