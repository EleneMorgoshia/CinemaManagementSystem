using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Ticket
    {
        private string _ticketCode;
        private decimal _finalPrice;

        public int Id { get; set; }
        public string TicketCode
        {
            get
            {
                return _ticketCode;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("TicketCode cannot be null or empty.");
                }
                _ticketCode = value;
            }
        }
        public decimal FinalPrice
        {
            get
            {
                return _finalPrice;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("FinalPrice cannot be negative.");
                }
                _finalPrice = value;
            }
        }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}

//Ticeket vs booking
// 1 ticket - 1 booking
// 1 booking - 1 ticket
//1 to 1
