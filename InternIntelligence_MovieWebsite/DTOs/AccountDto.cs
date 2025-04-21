using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternIntelligence_MovieWebsite.DTOs
{
    public class AccountDto
    {
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; } = string.Empty;
    }
}
