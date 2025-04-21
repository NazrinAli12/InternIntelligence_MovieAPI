using System;
using System.Collections.Generic;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int? GenreId { get; set; }

        public string? GenreName { get; set; }

        public double Rating { get; set; } = 0;
        public string? PosterUrl { get; set; }
    }
}
