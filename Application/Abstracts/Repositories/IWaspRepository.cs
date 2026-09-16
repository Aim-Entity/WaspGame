using Domain.Entities;

namespace Application.Abstracts.Repositories
{
    public interface IWaspRepository
    {
        public Task<IEnumerable<Wasp>> GetAllAsync();

        public Task<Wasp?> GetByIdAsync(long id);

        public Task<IEnumerable<Wasp>> GetRecoverableAsync();

        public Task<Wasp> UpdateAsync(Wasp wasp);

        public Task SaveChangesAsync();
    }
}
