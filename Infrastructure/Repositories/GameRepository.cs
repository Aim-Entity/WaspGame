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

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games
                 .Include(g => g.Wasps)
                 .AsNoTracking()
                 .ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(long id)
        {
            return await _context.Games
                 .Include(g => g.Wasps)
                 .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game?> GetCurrentAsync()
        {
            return await _context.Games
                 .Include(g => g.Wasps)
                 .OrderByDescending(g => g.Id)
                 .FirstOrDefaultAsync();
        }

        public async Task<Game> AddAsync(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task<Game> UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var game = await _context.Games
                .Include(g => g.Wasps)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
            {
                return false;
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
