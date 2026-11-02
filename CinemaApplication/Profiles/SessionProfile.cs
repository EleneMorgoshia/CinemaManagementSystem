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
    public class SessionProfile: Profile
    {
        public SessionProfile() 
        {
            //formamber მინდა იმისთის რომ მივწვდე ობიექტის ფროფერთის. Movie ფროფერთი session entity-ში მაქვს
            //როგორც ობიექტი და მაგის ფროფერთიებს რომ მივწვდე მჭირდება ეს formamber
            CreateMap<Session, SessionDTO>()
              .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
              .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.Movie.DurationMinutes))
              .ForMember(dest => dest.AgeRating,opt => opt.MapFrom(src => src.Movie.AgeRating))
              .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.HallName));

            CreateMap<CreateSessionDTO, Session>();
            CreateMap<UpdateSessionDTO, Session>();
        }
    }
}
