using Domain.Entities;
using Infrastructure.DatabaseContext;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests;

public sealed class GameRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly GameRepository _sut;

    public GameRepositoryTests()
    {
        // A database per test class instance, so tests never see each other's rows.
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"WaspGameTests-{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(_options);
        _sut = new GameRepository(_context);
    }
}
