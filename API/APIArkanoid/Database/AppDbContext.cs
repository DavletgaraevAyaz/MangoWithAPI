using APIArkanoid.Models;
using Microsoft.EntityFrameworkCore;

namespace APIArkanoid.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BallSkin> BallSkins { get; set; }
        public DbSet<UserBallSkin> UserBallSkins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBallSkin>()
                .HasKey(ubs => new { ubs.UserId, ubs.BallSkinId });

            // Seed initial data
            modelBuilder.Entity<BallSkin>().HasData(
                new BallSkin { Id = 1, Name = "Default", TexturePath = "default", Price = 0, IsDefault = true },
                new BallSkin { Id = 2, Name = "Fire", TexturePath = "fire", Price = 100 },
                new BallSkin { Id = 3, Name = "Ice", TexturePath = "ice", Price = 150 },
                new BallSkin { Id = 4, Name = "Galaxy", TexturePath = "galaxy", Price = 200 }
            );
        }
    }
}
