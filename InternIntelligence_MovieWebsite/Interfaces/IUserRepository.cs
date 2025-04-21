using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmailAsync(string userNameOrEmail);
        Task<bool> CreateUserAsync(User user);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> RestoreUserAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}
