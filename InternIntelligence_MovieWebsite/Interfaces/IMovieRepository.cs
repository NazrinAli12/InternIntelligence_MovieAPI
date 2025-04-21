using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<MovieEntity>> GetAllMoviesAsync();
        Task<MovieEntity?> GetMovieByNameAsync(string title); // Yeni metod
        Task<MovieEntity?> GetMovieByIdAsync(int id);
        Task<Movie?> FetchMovieFromApi(string title);

        Task<MovieEntity> AddMovieAsync(MovieEntity movie);
        Task<bool> UpdateMovieAsync(MovieEntity movie);
        Task<bool> SoftDeleteMovieAsync(int id);
        Task<bool> RestoreMovieAsync(int id);
        Task<bool> DeleteMovieAsync(int id);
    }
}
