using Application.Abstracts.Repositories;
using Domain.Entities;
using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WaspRepository : IWaspRepository
    {
        private readonly ApplicationDbContext _context;

        public WaspRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wasp>> GetAllAsync()
        {
            return await _context.Wasps
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Wasp?> GetByIdAsync(long id)
        {
            return await _context.Wasps.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Wasp>> GetRecoverableAsync()
        {
            var now = DateTimeOffset.UtcNow;

            return await _context.Wasps
                .Where(w => w.IsKnocked && w.KnockedUntil != null && w.KnockedUntil <= now)
                .ToListAsync();
        }

        public async Task<Wasp> UpdateAsync(Wasp wasp)
        {
            _context.Wasps.Update(wasp);
            await _context.SaveChangesAsync();

            return wasp;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
