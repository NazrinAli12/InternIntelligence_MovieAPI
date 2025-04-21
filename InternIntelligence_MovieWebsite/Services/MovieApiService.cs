using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Services
{
    public class MovieApiService : IMovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MovieApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey =
                configuration["TMDB:ApiKey"]
                ?? throw new Exception("TMDB API key could not found!");
        }

        public async Task<IEnumerable<MoviePopularDto>> GetPopularMoviesAsync()
        {
            var url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var movies = JsonSerializer.Deserialize<TmdbMovieResponse>(response, options);

            if (movies?.Results == null || !movies.Results.Any())
            {
                Console.WriteLine(" TMDB API-den bos netice geldi.");
                return new List<MoviePopularDto>();
            }

            var genres = await GetGenresAsync();

            return movies
                .Results.Select(movie => new MoviePopularDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Genre =
                        movie.GenreIds != null && movie.GenreIds.Any()
                            ? string.Join(
                                ", ",
                                movie.GenreIds.Select(id =>
                                    genres.ContainsKey(id) ? genres[id] : "Unknown"
                                )
                            )
                            : "Unknown",
                    ReleaseDate = movie.ReleaseDate,
                    Description = movie.Description,
                    PosterUrl = !string.IsNullOrWhiteSpace(movie.PosterPath)
                        ? $"https://image.tmdb.org/t/p/w500{movie.PosterPath}"
                        : null,
                    Rating = movie.Rating,
                })
                .ToList();
        }

        public async Task<MovieDetailsDto?> GetMovieDetailsAsync(int id)
        {
            var url = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=en-US";
            var response = await _httpClient.GetStringAsync(url);

            Console.WriteLine("TMDB API Response:");
            Console.WriteLine(response);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var movie = JsonSerializer.Deserialize<Movie>(response, options);

            if (movie is null)
                return null;

            var genres = await GetGenresAsync() ?? new Dictionary<int, string>();

            return new MovieDetailsDto
            {
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate,
                Description = movie.Description,
                PosterUrl = !string.IsNullOrWhiteSpace(movie.PosterPath)
                    ? $"https://image.tmdb.org/t/p/w500{movie.PosterPath}"
                    : null,
                Rating = movie.Rating,

                GenresList =
                    movie.GenresList != null && movie.GenresList.Any() ? movie.GenresList
                    : (movie.GenreIds != null && movie.GenreIds.Any())
                        ? movie
                            .GenreIds.Where(genres.ContainsKey)
                            .Select(id => new Genre { Id = id, Name = genres[id] })
                            .ToList()
                    : new List<Genre>(),
            };
        }

        public async Task<Dictionary<int, string>> GetGenresAsync()
        {
            var url =
                $"https://api.themoviedb.org/3/genre/movie/list?api_key={_apiKey}&language=en-US";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var genreResponse = JsonSerializer.Deserialize<TmdbGenreResponse>(response, options);

            if (genreResponse?.Genres is null || genreResponse.Genres.Count == 0)
            {
                Console.WriteLine("TMDB API-den janr siyahisi bos qayitdi.");
                return new Dictionary<int, string>();
            }
            Console.WriteLine(
                $"Janr siyahisi uğurla alindi: {genreResponse.Genres.Count} janr var."
            );

            return genreResponse
                .Genres.Where(x => !string.IsNullOrEmpty(x.Name)) // Null və ya boş olanları filtr et
                .ToDictionary(x => x.Id, x => x.Name!); // `!` operatoru ilə null olmamasını təsdiqlə
        }

        public async Task<IEnumerable<MoviePopularDto>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<MoviePopularDto>();

            query = Uri.EscapeDataString(query);
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={query}";

            var response = await _httpClient.GetStringAsync(url);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var movies = JsonSerializer.Deserialize<TmdbMovieResponse>(response, options);

            if (movies?.Results == null || !movies.Results.Any())
                return new List<MoviePopularDto>();

            var genres = await GetGenresAsync();

            return movies
                .Results.Select(movie => new MoviePopularDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Genre =
                        movie.GenreIds != null && movie.GenreIds.Any()
                            ? string.Join(
                                ", ",
                                movie.GenreIds.Select(id =>
                                    genres.ContainsKey(id) ? genres[id] : "Unknown"
                                )
                            )
                            : "Unknown",
                    ReleaseDate = movie.ReleaseDate,
                    Description = movie.Description,
                    PosterUrl = !string.IsNullOrWhiteSpace(movie.PosterPath)
                        ? $"https://image.tmdb.org/t/p/w500{movie.PosterPath}"
                        : null,
                    Rating = movie.Rating,
                })
                .ToList();
        }

        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            var url =
                $"https://api.themoviedb.org/3/movie/now_playing?api_key={_apiKey}&language=en-US&page=1";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var movies = JsonSerializer.Deserialize<TmdbMovieResponse>(response, options);

            if (movies?.Results == null || !movies.Results.Any())
            {
                Console.WriteLine(" TMDB API-dən boş nəticə gəldi.");
                return new List<MovieDto>();
            }

            var genres = await GetGenresAsync();

            return movies
                .Results.Select(movie => new MovieDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ReleaseDate = DateTime.TryParse(movie.ReleaseDate, out var date)
                        ? date
                        : DateTime.MinValue,

                    GenreId = movie.GenreIds?.FirstOrDefault() ?? 0, // ✅ ID-ləri olduğu kimi saxlayırıq.

                    GenreName =
                        movie.GenreIds != null && movie.GenreIds.Any() && genres != null
                            ? string.Join(
                                ", ",
                                movie.GenreIds.Where(genres.ContainsKey).Select(id => genres[id])
                            )
                            : "Unknown",

                    Rating = Math.Round(movie.Rating, 1), // ✨ 1 onluq dəqiqliklə yuvarlaqlaşdırır
                    PosterUrl = !string.IsNullOrWhiteSpace(movie.PosterPath)
                        ? $"https://image.tmdb.org/t/p/w500{movie.PosterPath}"
                        : null,
                })
                .ToList();
        }
    }
}
