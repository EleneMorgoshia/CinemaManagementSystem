using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class CreateSessionDTO
    {
        [Required]
        public DateTime StartsAt { get; set; }
        [Required]
        public decimal StandardPrice { get; set; }
        [Required]
        public decimal VipPrice { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
    }
}
