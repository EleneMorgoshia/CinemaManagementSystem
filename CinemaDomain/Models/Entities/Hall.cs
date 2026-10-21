using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Hall
    {
        private string _hallName;
        public int Id { get; set; }
        public string HallName
        {
            get
            {
                return _hallName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("HallName cannot be null or empty.");
                _hallName = value;
            }
        }

        //all seats that belong to a particualr hall(eg hall-A)
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        //All sessions scheduled in this particular hall
        public ICollection<Session> MovieSessions { get; set; } = new List<Session>();
    }
}


//Hall vs Seats+
//1 hall - Many seats 
//1 seat - 1 hall
//1 to many 
