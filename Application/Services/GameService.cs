using Application.Abstracts;
using Application.Abstracts.Repositories;
using Domain.Entities;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public Task<IEnumerable<Game>> GetGamesAsync()
        {
            return _gameRepository.GetAllAsync();
        }

        public Task<Game?> GetGameAsync(long id)
        {
            return _gameRepository.GetByIdAsync(id);
        }

        public async Task<Game> GetCurrentGameAsync()
        {
            var game = await _gameRepository.GetCurrentAsync();

            if (game is null)
            {
                return await CreateGameAsync();
            }

            if (game.Refresh())
            {
                await _gameRepository.UpdateAsync(game);
            }

            return game;
        }

        public Task<Game> CreateGameAsync()
        {
            return _gameRepository.AddAsync(Game.Create());
        }

        public Task<Game> ResetGameAsync()
        {
            return CreateGameAsync();
        }

        public async Task<ZapResult> ZapAsync()
        {
            var game = await _gameRepository.GetCurrentAsync() ?? await CreateGameAsync();

            return await ZapAsync(game);
        }

        public async Task<ZapResult?> ZapAsync(long gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);

            if (game is null)
            {
                return null;
            }

            return await ZapAsync(game);
        }

        public Task<bool> DeleteGameAsync(long id)
        {
            return _gameRepository.DeleteAsync(id);
        }

        public async Task RecoverWaspsAsync()
        {
            var game = await _gameRepository.GetCurrentAsync();

            if (game is not null && game.Refresh())
            {
                await _gameRepository.UpdateAsync(game);
            }
        }

        private async Task<ZapResult> ZapAsync(Game game)
        {
            var hit = game.Zap(Random.Shared);

            await _gameRepository.UpdateAsync(game);

            return new ZapResult(game, hit?.Id);
        }
    }
}
