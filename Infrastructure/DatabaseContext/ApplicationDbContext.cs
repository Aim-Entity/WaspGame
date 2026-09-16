using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Wasp> Wasps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>(game =>
            {
                game.HasKey(g => g.Id);
                game.HasMany(g => g.Wasps)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wasp>().HasKey(w => w.Id);
            modelBuilder.Entity<Queen>();
            modelBuilder.Entity<Drone>();
            modelBuilder.Entity<Worker>();
        }
    }
}
