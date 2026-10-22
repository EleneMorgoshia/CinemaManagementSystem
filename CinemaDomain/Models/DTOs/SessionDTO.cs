using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class SessionDTO
    {
        //Session info
        public int Id { get; set; }
        [Required]
        public DateTime StartsAt { get; set; }
        [Required]
        public decimal StandardPrice { get; set; }
        [Required]
        public decimal VipPrice { get; set; }

        //Movie info
        public int MovieId { get; set; }
        [Required]
        public string MovieTitle { get; set; }
        [Required]
        public int DurationMinutes { get; set; }
        [Required]
        public string AgeRating { get; set; }

        //Hall Info
        public int HallId { get; set; }
        [Required]
        public string HallName { get; set; }

    }
}
