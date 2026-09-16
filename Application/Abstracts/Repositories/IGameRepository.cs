using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IGameRepository
    {
        public Task<IEnumerable<Game>> GetAllAsync();

        public Task AddAsync(Game game);
    }
}
