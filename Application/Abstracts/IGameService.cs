using Domain.Entities;

namespace Application.Abstracts
{
    public interface IGameService
    {
        public Task<IEnumerable<Game>> GetGamesAsync();

        public Task<Game?> GetGameAsync(long id);

        public Task<Game> GetCurrentGameAsync();

        public Task<Game> CreateGameAsync();

        public Task<Game> ResetGameAsync();

        public Task<ZapResult> ZapAsync();

        public Task<ZapResult?> ZapAsync(long gameId);

        public Task<bool> DeleteGameAsync(long id);

        public Task RecoverWaspsAsync();
    }
}
