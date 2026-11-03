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
    public class HallProfile : Profile
    {
        public HallProfile() 
        {
            CreateMap<Hall, HallDTO>();
            CreateMap<CreateHallDTO, Hall>();
            CreateMap<UpdateHallDTO, Hall>();
            CreateMap<CreateHallDTO, HallDTO>();
        }
    }
}
