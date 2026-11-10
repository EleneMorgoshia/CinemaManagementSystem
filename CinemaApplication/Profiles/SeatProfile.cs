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
    public class SeatProfile : Profile
    {
        public SeatProfile()
        {
            CreateMap<Seat, SeatDTO>();
            CreateMap<CreateSeatDTO, Seat>();
            CreateMap<UpdateSeatDTO, Seat>();
            CreateMap<UpdateSeatDTO, Seat>()
                .ForMember(dest => dest.HallId, opt => opt.Ignore());
        }
    }
}
