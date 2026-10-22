using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class CreateHallDTO
    {
        [Required]
        [MaxLength(100)]
        public string HallName { get; set; }
    }
}
