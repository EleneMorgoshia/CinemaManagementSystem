using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int SessionId { get; set; }
        [Required]
        public int SeatId { get; set; }

    }
}
