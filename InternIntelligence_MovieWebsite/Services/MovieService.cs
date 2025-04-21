using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Data;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace InternIntelligence_MovieWebsite.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly HttpClient _httpClient;
        private readonly MovieDbContext _context;
        private readonly string _apiKey;

        public MovieService(
            IMovieRepository movieRepository,
            HttpClient httpClient,
            IConfiguration configuration,
            MovieDbContext context
        )
        {
            _movieRepository = movieRepository;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _context = context;
            _apiKey =
                configuration["TMDB:ApiKey"] ?? throw new Exception("TMDB API key is missing!");
        }

        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllMoviesAsync();
            return await Task.WhenAll(movies.Select(MapToDto));
        }

        public async Task<MovieDto?> GetMovieByNameAsync(string title)
        {
            var movie = await _movieRepository.GetMovieByNameAsync(title);

            if (movie is null)
            {
                var movieFromApi = await _movieRepository.FetchMovieFromApi(title);
                if (movieFromApi is null)
                    return null;

                int? genreId = await GetGenreIdFromApi(movieFromApi.GenreIds);

                var newMovie = new MovieEntity
                {
                    Title = movieFromApi.Title,
                    ReleaseDate = DateTime.TryParse(
                        movieFromApi.ReleaseDate,
                        out DateTime parsedDate
                    )
                        ? parsedDate
                        : (DateTime?)null,
                    GenreId = genreId,
                    Rating = movieFromApi.Rating,
                    PosterUrl = movieFromApi.PosterUrl,
                    IsDeleted = false,
                };

                Console.WriteLine($"Saving Movie: {newMovie.Title}, GenreId: {newMovie.GenreId}");
                var addedMovie = await _movieRepository.AddMovieAsync(newMovie);
                return await MapToDto(addedMovie);
            }

            return await MapToDto(movie);
        }

        private async Task<int?> GetGenreIdFromApi(List<int>? genreIds)
        {
            if (genreIds == null || !genreIds.Any())
                return null;

            await SyncGenresToDatabase();

            var genreId = await _context
                .Genres.Where(g => genreIds.Contains(g.Id))
                .Select(g => g.Id)
                .FirstOrDefaultAsync();

            return genreId != 0 ? genreId : null;
        }

        public async Task<(MovieDto?, string)> AddMovieAsync(MovieCreateAndUpdateDto movieDto)
        {
            var existingMovie = await _movieRepository.GetMovieByNameAsync(movieDto.Title);

            if (existingMovie is not null && !existingMovie.IsDeleted)
                return (null, "Movie already exists!");

            if (existingMovie is not null && existingMovie.IsDeleted)
            {
                existingMovie.IsDeleted = false;
                await _movieRepository.UpdateMovieAsync(existingMovie);
                return (await MapToDto(existingMovie), "Movie was restored successfully.");
            }

            if (!DateTime.TryParse(movieDto.ReleaseDate, out DateTime parsedDate))
                return (null, "Invalid date format! Please use 'YYYY-MM-DD'.");

            var newMovie = new MovieEntity
            {
                Title = movieDto.Title,
                ReleaseDate = parsedDate,
                GenreId = movieDto.GenreId,
                Rating = movieDto.Rating,
                PosterUrl = movieDto.PosterUrl,
                IsDeleted = false,
            };

            var addedMovie = await _movieRepository.AddMovieAsync(newMovie);
            return (await MapToDto(addedMovie), "Movie successfully created.");
        }

        public async Task<(bool, string)> UpdateMovieAsync(int id, MovieCreateAndUpdateDto movieDto)
        {
            var existingMovie = await _movieRepository.GetMovieByIdAsync(id);
            if (existingMovie is null)
                return (false, "Movie not found!");

            existingMovie.Title = movieDto.Title;

            if (DateTime.TryParse(movieDto.ReleaseDate, out DateTime parsedDate))
                existingMovie.ReleaseDate = parsedDate;
            else
                return (false, "Invalid date format! Use 'YYYY-MM-DD'.");

            existingMovie.GenreId = movieDto.GenreId;
            existingMovie.Rating = movieDto.Rating;
            existingMovie.PosterUrl = movieDto.PosterUrl;

            bool isUpdated = await _movieRepository.UpdateMovieAsync(existingMovie);
            return (isUpdated, isUpdated ? "Movie updated successfully!" : "Update failed.");
        }

        public async Task<bool> SoftDeleteMovieAsync(int id) =>
            await _movieRepository.SoftDeleteMovieAsync(id);

        public async Task<bool> RestoreMovieAsync(int id) =>
            await _movieRepository.RestoreMovieAsync(id);

        public async Task<bool> DeleteMovieAsync(int id) =>
            await _movieRepository.DeleteMovieAsync(id);

        private async Task<MovieDto> MapToDto(MovieEntity movie)
        {
            string? genreName = null;

            if (movie.GenreId.HasValue)
            {
                var genre = await _context.Genres.FindAsync(movie.GenreId);
                genreName = genre?.Name;
            }

            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate ?? default,
                GenreId = movie.GenreId ?? 0, // Null-dursa, 0 olaraq götür
                GenreName = genreName,
                Rating = movie.Rating,
                PosterUrl = movie.PosterUrl,
            };
        }

        private async Task SyncGenresToDatabase()
        {
            try
            {
                await LoadGenresFromApi();

                if (!_cachedGenres.Any())
                    return;

                foreach (var genre in _cachedGenres)
                {
                    var existingGenre = await _context.Genres.FindAsync(genre.Key);
                    if (existingGenre == null)
                    {
                        _context.Genres.Add(new Genre { Name = genre.Value }); // `Id` artıq avtomatik artır
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to sync genres to database: {ex.Message}");
            }
        }

        private async Task LoadGenresFromApi()
        {
            if ((DateTime.UtcNow - _lastCacheUpdate).TotalHours < 6 && _cachedGenres.Any())
                return;

            try
            {
                var genreUrl =
                    $"https://api.themoviedb.org/3/genre/movie/list?api_key={_apiKey}&language=en-US";
                var response = await _httpClient.GetStringAsync(genreUrl);

                if (!string.IsNullOrEmpty(response))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var genreData = JsonSerializer.Deserialize<TmdbGenreResponse>(
                        response,
                        options
                    );

                    if (genreData?.Genres != null && genreData.Genres.Any())
                    {
                        _cachedGenres = genreData.Genres.ToDictionary(g => g.Id, g => g.Name);
                        _lastCacheUpdate = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to fetch genres from TMDB: {ex.Message}");
            }
        }

        private static Dictionary<int, string> _cachedGenres = new();
        private static DateTime _lastCacheUpdate = DateTime.MinValue;
    }
}
