using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class CreateSeatDTO
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int RowNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SeatNumber { get; set; }
        [Required]
        public bool IsVip { get; set; }
        [Range(1, int.MaxValue)]
        [Required]
        public int HallId { get; set; }
    }
}
