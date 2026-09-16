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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>(game =>
            {
                game.HasKey(g => g.Id);
                game.HasMany(g => g.Wasps)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

                game.Ignore(g => g.KnockoutSeconds);
            });

            modelBuilder.Entity<Wasp>(wasp =>
            {
                wasp.HasKey(w => w.Id);

                wasp.Ignore(w => w.Type);
            });

            modelBuilder.Entity<Queen>();
            modelBuilder.Entity<Drone>();
            modelBuilder.Entity<Worker>();
        }
    }
}
