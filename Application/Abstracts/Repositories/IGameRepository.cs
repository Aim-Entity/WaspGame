using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IGameRepository
    {
        public Task<Game?> GetCurrentAsync(CancellationToken cancellationToken = default);

        public Task<Game> ReplaceCurrentAsync(Game game, CancellationToken cancellationToken = default);

        public Task<Game> UpdateAsync(Game game, CancellationToken cancellationToken = default);
    }
}
