using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task<MovieDto?> GetMovieByNameAsync(string title); // Yeni metod
        Task<(MovieDto?, string)> AddMovieAsync(MovieCreateAndUpdateDto movieDto);
        Task<(bool, string)> UpdateMovieAsync(int id, MovieCreateAndUpdateDto movieDto); // Dəyişdirildi!
        Task<bool> SoftDeleteMovieAsync(int id);
        Task<bool> RestoreMovieAsync(int id);
        Task<bool> DeleteMovieAsync(int id);
    }
}
