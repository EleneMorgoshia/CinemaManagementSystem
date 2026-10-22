using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class UpdateSessionDTO
    {
        [Required]
        public DateTime StartsAt { get; set; }
        [Required]
        public decimal StandardPrice { get; set; }
        [Required]
        public decimal VipPrice { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required]
        public int HallId { get; set; }
    }
}
