using Application.Abstracts;
using Application.Abstracts.Repositories;
using Domain.Entities;

namespace Application.Services
{
    public class WaspService : IWaspService
    {
        private readonly IWaspRepository _waspRepository;

        public WaspService(IWaspRepository waspRepository)
        {
            _waspRepository = waspRepository;
        }

        public Task<IEnumerable<Wasp>> GetWaspsAsync()
        {
            return _waspRepository.GetAllAsync();
        }

        public Task<Wasp?> GetWaspAsync(long id)
        {
            return _waspRepository.GetByIdAsync(id);
        }

        public async Task<Wasp?> ZapWaspAsync(long id)
        {
            var wasp = await _waspRepository.GetByIdAsync(id);

            if (wasp is null)
            {
                return null;
            }

            wasp.RecoverIfDue();
            wasp.TakeDamage(Game.ZapDamage, Game.KnockoutDuration);

            return await _waspRepository.UpdateAsync(wasp);
        }

        public async Task<Wasp?> RecoverWaspAsync(long id)
        {
            var wasp = await _waspRepository.GetByIdAsync(id);

            if (wasp is null)
            {
                return null;
            }

            wasp.Recover();

            return await _waspRepository.UpdateAsync(wasp);
        }

        public async Task<int> RecoverKnockedWaspsAsync()
        {
            var wasps = (await _waspRepository.GetRecoverableAsync()).ToList();

            foreach (var wasp in wasps)
            {
                wasp.Recover();
            }

            if (wasps.Count > 0)
            {
                await _waspRepository.SaveChangesAsync();
            }

            return wasps.Count;
        }
    }
}
