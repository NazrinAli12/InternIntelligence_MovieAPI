using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class MoviePopularDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("genre")]
        public string? Genre { get; set; }

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
    }
}
