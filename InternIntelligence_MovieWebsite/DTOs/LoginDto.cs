using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username or email cannot be empty!")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty!")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
        public string Password { get; set; } = string.Empty;
    }
}
