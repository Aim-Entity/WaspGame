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
                 .AsNoTracking()
                 .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> AddAsync(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return game;
        }
    }
}
