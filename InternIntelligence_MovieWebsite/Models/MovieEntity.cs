using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternIntelligence_MovieWebsite.Models
{
    [Table("Movies")]
    public class MovieEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public int? GenreId { get; set; } // ✅ Store GenreId

        public DateTime? ReleaseDate { get; set; }
        public double Rating { get; set; } = 0;
        public string? PosterUrl { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; } // ✅ Navigation Property
    }
}
