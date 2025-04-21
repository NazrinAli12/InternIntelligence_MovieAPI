using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;
using InternIntelligence_MovieWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternIntelligence_MovieWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IMovieApiService _movieApiService;

        public MovieController(IMovieService movieService, IMovieApiService movieApiService)
        {
            _movieService = movieService;
            _movieApiService = movieApiService;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies()
        {
            var movies = await _movieApiService.GetPopularMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            var movie = await _movieApiService.GetMovieDetailsAsync(id);
            if (movie is null)
                return NotFound("Movie not found.");

            return Ok(movie);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var movies = await _movieApiService.SearchMoviesAsync(query);
            return Ok(movies);
        }

        // For Local database
        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieApiService.GetAllMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("name/{title}")]
        public async Task<IActionResult> GetMovieByName(string title)
        {
            var movie = await _movieService.GetMovieByNameAsync(title);
            if (movie is null)
                return NotFound($"Movie with title '{title}' not found.");
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] MovieCreateAndUpdateDto movieDto)
        {
            var (addedMovie, message) = await _movieService.AddMovieAsync(movieDto);

            if (addedMovie is null)
                return BadRequest(new { message }); // Əgər film artıq varsa və ya tarix səhvdirsə, mesajı qaytarırıq

            return CreatedAtAction(
                nameof(GetMovieByName),
                new { title = addedMovie.Title },
                new { message, addedMovie }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(
            int id,
            [FromBody] MovieCreateAndUpdateDto movieDto
        )
        {
            var (updated, message) = await _movieService.UpdateMovieAsync(id, movieDto);

            if (!updated)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteMovie(int id)
        {
            var result = await _movieService.SoftDeleteMovieAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreMovie(int id)
        {
            var result = await _movieService.RestoreMovieAsync(id);
            if (!result)
                return NotFound("Movie not found or is already active.");

            return NoContent();
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
