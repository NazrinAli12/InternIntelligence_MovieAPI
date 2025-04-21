using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternIntelligence_MovieWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace InternIntelligence_MovieWebsite.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options) { }

        public DbSet<MovieEntity> Movies { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MovieEntity>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(m => !m.IsDeleted);

            modelBuilder
                .Entity<MovieEntity>()
                .Property(m => m.ReleaseDate)
                .HasConversion(
                    v => v.Value.ToString("yyyy-MM-dd"), //datetime to string
                    v => DateTime.Parse(v) //string to datetime
                );
        }
    }
}
