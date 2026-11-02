using CinemaDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class CreateMovieDTO
    {
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


        //ჟანრის აიდები გავატანოთ ნებისმიერ ფილმს დამატების დროს
        [Required]
        [MinLength(1, ErrorMessage = "Movie must have at least one genre.")]
        public List<int> GenreIds { get; set; } = new();
    }
}
