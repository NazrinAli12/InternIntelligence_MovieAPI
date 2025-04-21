using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.DTOs;
using InternIntelligence_MovieWebsite.Interfaces;
using InternIntelligence_MovieWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InternIntelligence_MovieWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AccountController(IUserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (
                string.IsNullOrWhiteSpace(loginDto.UserNameOrEmail)
                || string.IsNullOrWhiteSpace(loginDto.Password)
            )
                return BadRequest(new { message = "Username/Email or password cannot be empty." });

            var user = await _userService.GetUserByUsernameorEmailAsync(
                loginDto.UserNameOrEmail,
                loginDto.Password
            );

            if (user is null)
                return Unauthorized("Invalid username or password.");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isCreated = await _userService.CreateUserAsync(registerDto);
            if (!isCreated)
                return BadRequest(
                    new { message = "User with this email or username already exists." }
                );
            return Created("", new { message = "User registered successfully." });
        }

        [Authorize]
        [HttpPut("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            var result = await _userService.SoftDeleteUserAsync(id);
            if (!result)
                return NotFound(new { message = "User not found or already deleted." });

            return Ok(new { message = "User soft deleted successfully." });
        }

        [Authorize]
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var result = await _userService.RestoreUserAsync(id);
            if (!result)
                return NotFound(new { message = "User not found or already active." });

            return Ok(new { message = "User restored successfully." });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [Authorize]
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound(new { nessage = "User not found." });

            return Ok(new { message = "User permanently deleted." });
        }
    }
}
