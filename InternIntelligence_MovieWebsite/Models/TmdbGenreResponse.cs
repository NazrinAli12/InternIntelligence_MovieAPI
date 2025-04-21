using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.Models
{
    public class TmdbGenreResponse
    {
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();
    }
}