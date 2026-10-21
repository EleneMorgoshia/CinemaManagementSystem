using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class MovieGenre
    {
        public int MovieId { get; set; } // Foreign Key
        public int GenreId { get; set; } // Foreign key

        public Movie Movie { get; set; } // movie navigation property
        public Genre Genre { get; set; } // genre navigation property 
    }
}

//MovieGenre vs Movie
//MovieGenre vs Genre
