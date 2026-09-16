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

        public async Task<Game> GetCurrentGameAsync()
        {
            var game = await _gameRepository.GetCurrentAsync();

            if (game is null)
            {
                return await ResetGameAsync();
            }

            if (game.Refresh())
            {
                await _gameRepository.UpdateAsync(game);
            }

            return game;
        }

        public Task<Game> ResetGameAsync()
        {
            return _gameRepository.AddAsync(Game.Create());
        }

        public async Task<ZapResult> ZapAsync()
        {
            var game = await _gameRepository.GetCurrentAsync() ?? await ResetGameAsync();
            var hit = game.Zap(Random.Shared);

            await _gameRepository.UpdateAsync(game);

            return new ZapResult(game, hit?.Id);
        }
    }
}
