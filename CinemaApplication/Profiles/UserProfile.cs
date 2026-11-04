using AutoMapper;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CinemaApplication.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDTO>();

        }
    }
}
