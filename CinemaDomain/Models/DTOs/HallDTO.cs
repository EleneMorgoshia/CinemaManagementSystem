using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class HallDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string HallName { get; set; }
    }
}
