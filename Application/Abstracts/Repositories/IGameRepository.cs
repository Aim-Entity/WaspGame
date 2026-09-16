using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IGameRepository
    {
        public Task<IEnumerable<Game>> GetAllAsync();

        public Task<Game?> GetByIdAsync(long id);

        public Task<Game> AddAsync(Game game);
    }
}
