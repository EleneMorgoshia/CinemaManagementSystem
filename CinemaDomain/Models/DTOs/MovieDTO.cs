using CinemaDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Range(1, 600)]
        public int DurationMinutes { get; set; }
        [Required]
        [EnumDataType(typeof(AgeRating))]
        public AgeRating AgeRating { get; set; }
        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
