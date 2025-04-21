using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Interfaces
{
    public interface IUserService
    {
        Task<User?> AutehnticateUserAsync(string userNameOrEmail, string password);
        Task<User?> GetUserByUsernameorEmailAsync(string userNameOrEmail, string password);
        Task<bool> CreateUserAsync(RegisterDto registerDto);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> RestoreUserAsync(int id);
        Task<bool> DeleteUserAsync(int id); //hard delete.
    }
}
