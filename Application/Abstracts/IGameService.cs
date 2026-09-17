using Domain.Entities;

namespace Application.Abstracts
{
    public interface IGameService
    {
        public Task<Game> GetOrCreateCurrentGameAsync(CancellationToken cancellationToken = default);

        public Task<Game> ResetGameAsync(CancellationToken cancellationToken = default);

        public Task<ZapResult> ZapAsync(CancellationToken cancellationToken = default);
    }
}
