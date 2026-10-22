using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }
        [Required]
        public string TicketCode { get; set; }
        [Required]
        public decimal FinalPrice { get; set; }
        public int BookingId { get; set; }
    }
}
