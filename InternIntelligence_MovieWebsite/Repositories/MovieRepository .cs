using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Data;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace InternIntelligence_MovieWebsite.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MovieRepository(
            MovieDbContext context,
            HttpClient httpClient,
            IConfiguration configuration
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient;
            _apiKey =
                configuration["TMDB:ApiKey"] ?? throw new Exception("TMDB API key is missing!");
        }

        public async Task<IEnumerable<MovieEntity>> GetAllMoviesAsync()
        {
            return await _context.Movies.Where(m => !m.IsDeleted).ToListAsync();
        }

        public async Task<MovieEntity?> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovieEntity?> GetMovieByNameAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var existingMovie = await _context
                .Movies.IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Title == title);

            return existingMovie;
        }

        public async Task<Movie?> FetchMovieFromApi(string title)
        {
            var url =
                $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(title)}&language=en-US";

            var response = await _httpClient.GetStringAsync(url);
            Console.WriteLine($"TMDB Response: {response}"); // ✅ Debugging TMDB response

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var searchResult = JsonSerializer.Deserialize<MovieSearchResult>(response, options);

            return searchResult?.Results?.FirstOrDefault();
        }

        public async Task<MovieEntity> AddMovieAsync(MovieEntity movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> UpdateMovieAsync(MovieEntity movie)
        {
            _context.Movies.Update(movie);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie is null || movie.IsDeleted)
                return false;

            movie.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RestoreMovieAsync(int id)
        {
            var movie = await _context
                .Movies.IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie is null || !movie.IsDeleted)
                return false;

            movie.IsDeleted = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie is null)
                return false;

            _context.Movies.Remove(movie);
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public class MovieSearchResult
    {
        public List<Movie> Results { get; set; } = new();
    }
}
