using CinemaDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get;set;  }
        [Required]
        public string Email {  get; set; }
        [Required]
        public Role Role {  get; set; }
    }
}
