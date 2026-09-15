using Application.Abstracts.Repositories;
using Domain.Entities;
using Infrastructure.DatabaseContext;

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
            return _context.Games;
        }
    }
}
