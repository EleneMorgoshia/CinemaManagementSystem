using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        //booking vs user relation
        public int UserId { get; set; } // foreign key to User
        public User User { get; set; }//navigation property
        
        //booking vs session
        public int SessionId { get; set; }//foreign key to session
        public Session Session { get; set; }

        //bookign vs seat
        public int SeatId { get; set; }
        public Seat Seat { get; set; }

        //booking vs ticket
        public Ticket Ticket { get; set; }
        
    }
}


//booking vs User
//1 User - Many booking
//1 booking - 1user
//1 to many


//booking vs session
//1 seesion - Many booking
//1 booking - one session - თითოეული ჯავშანი კონკრეტულ სეანსს უკავშირდება
//ანუ ახალი ჯავშანი მინდა თუ ახალ სეანსზე მივდივარ
//1 To Many


//booking vs seat
//1 booking - 1 seat
//1 seat - Many booking
//Many TO many

//booking vs tickets
//1 booking - 1 ticket
//1 ticket - 1 booking
//1 to one