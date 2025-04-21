using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class MovieDetailsDto
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        public int ReleaseYear =>
            !string.IsNullOrWhiteSpace(ReleaseDate) ? int.Parse(ReleaseDate[..4]) : 0;

        [JsonPropertyName("overview")]
        public string? Description { get; set; }

        [JsonPropertyName("vote_average")]
        public double Rating { get; set; } = 0;

        [JsonPropertyName("posterUrl")]
        public string? PosterUrl { get; set; }

        [JsonPropertyName("genres")]
        public List<Genre>? GenresList { get; set; }
    }
}
