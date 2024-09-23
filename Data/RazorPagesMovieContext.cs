using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Data
{
    public class RazorPagesMovieContext : DbContext
    {
        public RazorPagesMovieContext (DbContextOptions<RazorPagesMovieContext> options)
            : base(options)
        {
        }

        public DbSet<RazorPagesMovie.Models.Movie> Movie { get; set; } = default!;
        public DbSet<RazorPagesMovie.Models.User> User { get; set; } = default!;
        public DbSet<RazorPagesMovie.Models.Course> Course { get; set; } = default!;
        public DbSet<RazorPagesMovie.Models.Enrollment> Enrollment { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>().HasKey(e => e.Id);
            modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
            modelBuilder.Entity<Enrollment>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<Enrollment>().Property(e => e.CourseId).IsRequired();   

            base.OnModelCreating(modelBuilder);
        }
    }
}
