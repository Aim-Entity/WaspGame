using Application.Abstracts.Repositories;
using Domain.Entities;
using Infrastructure.DatabaseContext;

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
            return _context.Wasps;
        }
    }
}
