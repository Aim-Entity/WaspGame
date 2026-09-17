using Application.Abstracts;
using Application.Abstracts.Repositories;
using Domain.Entities;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly TimeProvider _timeProvider;
        private readonly Random _random;

        public GameService(IGameRepository gameRepository, TimeProvider timeProvider, Random random)
        {
            _gameRepository = gameRepository;
            _timeProvider = timeProvider;
            _random = random;
        }

        public async Task<Game> GetOrCreateCurrentGameAsync(CancellationToken cancellationToken = default)
        {
            var now = _timeProvider.GetUtcNow();
            var game = await _gameRepository.GetCurrentAsync(cancellationToken);

            if (game is null)
            {
                return await ResetGameAsync(cancellationToken);
            }

            if (game.Refresh(now))
            {
                await _gameRepository.UpdateAsync(game, cancellationToken);
            }

            return game;
        }

        public Task<Game> ResetGameAsync(CancellationToken cancellationToken = default)
        {
            return _gameRepository.ReplaceCurrentAsync(Game.Create(), cancellationToken);
        }

        public async Task<ZapResult> ZapAsync(CancellationToken cancellationToken = default)
        {
            var now = _timeProvider.GetUtcNow();
            var game = await _gameRepository.GetCurrentAsync(cancellationToken);

            if (game is null)
            {
                // Zap before persisting so a first-ever zap is a single write, not an add followed by an update.
                var created = Game.Create();
                var firstOutcome = created.Zap(_random, now);
                var persisted = await _gameRepository.ReplaceCurrentAsync(created, cancellationToken);

                return new ZapResult(persisted, firstOutcome.Hit?.Id);
            }

            var outcome = game.Zap(_random, now);

            if (outcome.StateChanged)
            {
                await _gameRepository.UpdateAsync(game, cancellationToken);
            }

            return new ZapResult(game, outcome.Hit?.Id);
        }
    }
}
