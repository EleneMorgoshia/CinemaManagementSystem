using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Seat
    {
        private int _rowNumber;
        private int _seatNumber;
        public int Id { get; set; }
        public int RowNumber
        {
            get
            {
                return _rowNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("RowNumber must be greater than 0.");
                _rowNumber = value;
            }
        }
        public int SeatNumber
        {
            get
            {
                return _seatNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("SeatNumber must be greater than 0.");
                _seatNumber = value;
            }
        }
        public bool IsVip { get; set; }

        public int HallId { get; set; }// foreign key for hall
        public Hall Hall { get; set; } //navigation property 

        //1 seat - many booking
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        
    }
}


