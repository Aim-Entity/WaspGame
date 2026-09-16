using Domain.Entities;

namespace Application.Abstracts
{
    public interface IGameService
    {
        public Task<Game> GetCurrentGameAsync();

        public Task<Game> ResetGameAsync();

        public Task<ZapResult> ZapAsync();
    }
}
