using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Data;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace InternIntelligence_MovieWebsite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MovieDbContext _context;

        public UserRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameOrEmailAsync(string userNameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(userNameOrEmail))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u =>
                (u.Email == userNameOrEmail || u.UserName == userNameOrEmail) && !u.IsDeleted
            );
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            if (
                await _context.Users.AnyAsync(u =>
                    u.Email == user.Email || u.UserName == user.UserName
                )
            )
                return false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null || user.IsDeleted)
                return false;

            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreUserAsync(int id)
        {
            var user = await _context
                .Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted);

            if (user is null)
                return false;

            user.IsDeleted = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
