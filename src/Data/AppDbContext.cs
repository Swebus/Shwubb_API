using ShwubbApi.Models;
using System.Linq;
using ShwubbApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;

namespace ShwubbApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<ShwubbUser> Users { get; set; }
        public DbSet<ShwubbPost> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShwubbPost>()
                .HasOne(p => p.Author)
                .WithMany(p => p.Posts)
                .HasForeignKey(p => p.Userid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Read connection string from configuration
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Connection string 'DefaultConnection' is not set.");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

}
