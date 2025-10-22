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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;AttachDbFilename=C:\Users\Sebastian\Shwubb.mdf;");
        }
    }
}
