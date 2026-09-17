using Application.Abstracts.Repositories;
using Domain.Entities;
using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetCurrentAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Games
                 .Include(g => g.Wasps)
                 .Where(g => g.IsActive)
                 .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Game> ReplaceCurrentAsync(Game game, CancellationToken cancellationToken = default)
        {
            var active = await _context.Games
                .Where(g => g.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var previous in active)
            {
                previous.Deactivate();
            }

            await _context.Games.AddAsync(game, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return game;
        }

        public async Task<Game> UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            if (_context.Entry(game).State == EntityState.Detached)
            {
                _context.Games.Update(game);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return game;
        }
    }
}
