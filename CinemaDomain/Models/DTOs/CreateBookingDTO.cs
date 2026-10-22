using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class CreateBookingDTO
    {
        [Required]
        public int SessionId { get; set; }
        [Required]
        public int SeatId { get; set; }
    }
}
