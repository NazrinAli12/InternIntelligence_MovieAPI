using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.Models
{
    public class TmdbMovieResponse
    {
        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; } = new();
    }
}