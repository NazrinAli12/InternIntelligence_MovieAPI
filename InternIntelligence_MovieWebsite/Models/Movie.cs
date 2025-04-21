using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Core.Serialization;

namespace InternIntelligence_MovieWebsite.Models
{
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }

        public string? Genre { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        public int ReleaseYear =>
            !string.IsNullOrWhiteSpace(ReleaseDate) ? int.Parse(ReleaseDate[..4]) : 0;

        [JsonPropertyName("overview")]
        public string? Description { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double Rating { get; set; } = 0;
        public string? PosterUrl =>
            !string.IsNullOrWhiteSpace(PosterPath)
                ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
                : null;

        //Soft delete
        public bool IsDeleted { get; set; } = false;

        [JsonPropertyName("genres")]
        public List<Genre>? GenresList { get; set; }
    }
}
