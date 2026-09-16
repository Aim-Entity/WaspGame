using Domain.Entities;

namespace Application.Abstracts
{
    public interface IWaspService
    {
        public Task<IEnumerable<Wasp>> GetWaspsAsync();

        public Task<Wasp?> GetWaspAsync(long id);

        public Task<Wasp?> ZapWaspAsync(long id);

        public Task<Wasp?> RecoverWaspAsync(long id);

        public Task<int> RecoverKnockedWaspsAsync();
    }
}
