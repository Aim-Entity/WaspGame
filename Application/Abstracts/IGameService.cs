using Domain.Entities;

namespace Application.Abstracts
{
    public interface IGameService
    {
        public Task<Game> GetOrCreateCurrentGameAsync(CancellationToken cancellationToken = default);

        public Task<Game> ResetGameAsync(CancellationToken cancellationToken = default);

        public Task<ZapResult> ZapAsync(CancellationToken cancellationToken = default);
    }

    public sealed class ZapResult
    {
        public ZapResult(Game game, long? hitWaspId)
        {
            Game = game;
            HitWaspId = hitWaspId;
        }

        public Game Game { get; }

        public long? HitWaspId { get; }
    }
}
