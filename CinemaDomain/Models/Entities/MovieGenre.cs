using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class MovieGenre
    {
        public int MovieId { get; set; } 
        public int GenreId { get; set; } 

        public Movie Movie { get; set; } 
        public Genre Genre { get; set; } 
    }
}

//MovieGenre vs Movie
//MovieGenre vs Genre
