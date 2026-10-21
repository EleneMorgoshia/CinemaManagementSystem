using CinemaDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Movie
    {
        private string _title;
        private AgeRating _ageRating;
        private int _durationMinutes;
        public int Id { get; set; }
        public string Title
        {
            get 
            { 
                return _title; 
            }
            set 
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Title cannot be null or empty.");
                }
                _title = value;
            }   
        }

        public int DurationMinutes
        {
            get
            {
                return _durationMinutes;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Duration must be greater than 0.");
                }
                _durationMinutes = value;   
            }
        }
        public AgeRating AgeRating
        {
            get
            {
                return _ageRating;
            }
            set
            {
                if(!Enum.IsDefined(typeof(AgeRating), value))
                    throw new ArgumentException("Invalid age rating.");
                _ageRating = value;
            }
        }

        public string? Description { get; set; }

        //1 particular movie and all the records connected to it.
        public ICollection<MovieGenre> MovieGenreRecords { get; set; } = new List<MovieGenre>();

        //Sessions that can have 1 particualr movie
        public ICollection<Session> MovieSessions { get; set; } = new List<Session>();
    }
}

//Movie and MovieGnere +
// 1 movie - Many Genre records
// 1genre recode - 1 movie

//Movie and Session +