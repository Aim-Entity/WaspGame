using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IGameRepository
    {
        public Task<Game?> GetCurrentAsync();

        public Task<Game> AddAsync(Game game);

        public Task<Game> UpdateAsync(Game game);
    }
}
