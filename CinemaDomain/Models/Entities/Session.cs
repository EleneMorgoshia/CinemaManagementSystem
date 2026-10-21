using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Session
    {
        private decimal _standardPrice;
        private decimal _VipPrice;
        public int Id { get; set; }
        public int MovieId { get; set; } // foreign key for the movie
        public int HallId { get; set; }//foreign key for the hall
        public DateTime StartsAt { get; set; }
        public decimal StandardPrice
        {
            get
            {
                return _standardPrice;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Standard price cannot be negative.");
                _standardPrice = value;
            }
        }
        public decimal VipPrice
        {
            get
            {
                return _VipPrice;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("VIP price cannot be negative.");
                _VipPrice = value;
            }
        }
        public bool Status { get; set; }

        public Movie Movie { get; set; }
        public Hall Hall { get; set; }

        //Session vs Movies
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

//Session vs Movie +
//1 session - 1 MOVIE
//1MOVIE - MANY SESSION
//1 TO MANY


//session vs hall +
//1 SESSION - 1 HALL
//1HALL - Many SESSION
//1 TO MANY

//Session vs booking+
