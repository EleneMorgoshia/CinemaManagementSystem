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
    public class BookingProfile : Profile
    {
        public BookingProfile() 
        {
            CreateMap<Booking, BookingDTO>();
            CreateMap<CreateBookingDTO, Booking>();
        }
    }
}
