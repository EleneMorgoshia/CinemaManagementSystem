using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class UpdateSeatDTO
    {
        [Required]
        public bool IsVip { get; set; }
    }
}
