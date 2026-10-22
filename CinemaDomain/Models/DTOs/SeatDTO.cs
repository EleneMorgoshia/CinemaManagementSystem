using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class SeatDTO
    {
        public int Id { get; set; }
        [Required]
        public int RowNumber { get; set; }
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        public bool IsVip { get; set; }
        [Required]
        public int HallId { get; set; }
    }
}
