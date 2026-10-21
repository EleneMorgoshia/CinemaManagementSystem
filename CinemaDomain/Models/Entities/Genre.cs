using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string GenreName { get; set; }


        //1 Genre and all the records connected to it
        public ICollection<MovieGenre> MovieGenreRecords { get; set; } = new List<MovieGenre>();
    }
}

//Genre vs MovieGenreRecords+