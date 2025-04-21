using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;

namespace InternIntelligence_MovieWebsite.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByUsernameorEmailAsync(
            string userNameOrEmail,
            string password
        )
        {
            if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetUserByUsernameOrEmailAsync(userNameOrEmail);
            if (user is null || user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }

        public async Task<bool> CreateUserAsync(RegisterDto registerDto)
        {
            var hashedPassword = HashPassword(registerDto.Password);

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
            };

            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            return await _userRepository.SoftDeleteUserAsync(id);
        }

        public async Task<bool> RestoreUserAsync(int id)
        {
            return await _userRepository.RestoreUserAsync(id);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        public async Task<User?> AutehnticateUserAsync(string userNameOrEmail, string password)
        {
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(userNameOrEmail);

            if (user is null || user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }
    }
}
