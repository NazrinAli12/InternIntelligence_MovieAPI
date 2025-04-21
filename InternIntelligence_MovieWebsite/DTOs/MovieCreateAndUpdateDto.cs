using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class MovieCreateAndUpdateDto
    {
        public string? Title { get; set; }
        public string? ReleaseDate { get; set; }
        public int? GenreId { get; set; }
        public double Rating { get; set; } = 0;
        public string? PosterUrl { get; set; }
    }
}
