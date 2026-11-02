using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.DTOs
{
    public class MovieGenreDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Movie ID must be greater than 0.")]
        public int MovieId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Genre ID must be greater than 0.")]
        public int GenreId { get; set; }
    }
}
