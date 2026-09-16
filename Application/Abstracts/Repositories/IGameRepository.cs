using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IGameRepository
    {
        public Task<IEnumerable<Game>> GetAllAsync();

        public Task<Game?> GetByIdAsync(long id);

        public Task<Game?> GetCurrentAsync();

        public Task<Game> AddAsync(Game game);

        public Task<Game> UpdateAsync(Game game);

        public Task<bool> DeleteAsync(long id);
    }
}
