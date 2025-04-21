using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Interfaces
{
    public interface IMovieApiService
    {
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task<IEnumerable<MoviePopularDto>> GetPopularMoviesAsync();
        Task<MovieDetailsDto?> GetMovieDetailsAsync(int id);
        Task<IEnumerable<MoviePopularDto>> SearchMoviesAsync(string query);
        Task<Dictionary<int, string>> GetGenresAsync();
    }
}
