using Application.Abstracts.Repositories;
using Application.Services;
using Application.Utils;
using Domain;
using Domain.Entities;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Application.UnitTests.Services;

public class GameServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly Mock<IGameRepository> _repository = new(MockBehavior.Strict);
    private readonly FakeTimeProvider _timeProvider = new(Now);
    private readonly Mock<Random> _random = new();

    public GameServiceTests()
    {
        _random.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
    }

    private GameService CreateSut() => new(_repository.Object, _timeProvider, _random.Object);

    private static Wasp Knocked(Wasp wasp, DateTimeOffset? knockedUntil)
    {
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = knockedUntil;

        return wasp;
    }

    // ---------------------------------------------------------------------
    // GameService.GetOrCreateCurrentGameAsync
    // ---------------------------------------------------------------------

    [Fact]
    public async Task GameService_GetOrCreateCurrentGameAsync_WhenNoGameHasEverBeenStarted_CreatesAndPersistsAFreshGame()
    {
        // ARRANGE
        Game? persisted = null;
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);
        _repository.Setup(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .Callback<Game, CancellationToken>((game, _) => persisted = game)
            .ReturnsAsync((Game game, CancellationToken _) => game);
        var sut = CreateSut();

        // ACT
        var result = await sut.GetOrCreateCurrentGameAsync();

        // ASSERT
        Assert.NotNull(persisted);
        Assert.Same(persisted, result);
        Assert.Equal(13, result.Wasps.Count);
        Assert.True(result.IsActive);
        Assert.False(result.IsGameDone);
        _repository.Verify(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GameService_GetOrCreateCurrentGameAsync_WhenTheStoredGameIsAlreadyUpToDate_ReturnsItWithoutWritingToTheRepository()
    {
        // ARRANGE
        var stored = GameLogic.Create();
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.GetOrCreateCurrentGameAsync();

        // ASSERT
        Assert.Same(stored, result);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GameService_GetOrCreateCurrentGameAsync_WhenAKnockedWaspIsDueToRecover_RecoversItAndPersistsTheChangeOnce()
    {
        // ARRANGE
        var drone = Knocked(new Drone(), Now.AddSeconds(-1));
        var stored = new Game { Wasps = new List<Wasp> { new Queen(), drone } };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.GetOrCreateCurrentGameAsync();

        // ASSERT
        Assert.Same(stored, result);
        Assert.False(drone.IsKnocked);
        Assert.Equal(60, drone.Energy);
        _repository.Verify(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GameService_GetOrCreateCurrentGameAsync_WhenTheDeadlineHasNotPassedOnTheInjectedClock_LeavesTheWaspKnockedAndSkipsTheWrite()
    {
        // ARRANGE
        // The deadline is in the past by wall-clock time but still in the future for the injected
        // clock, so this only passes if the service reads TimeProvider rather than DateTimeOffset.UtcNow.
        var drone = Knocked(new Drone(), Now.AddSeconds(1));
        var stored = new Game { Wasps = new List<Wasp> { new Queen(), drone } };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.GetOrCreateCurrentGameAsync();

        // ASSERT
        Assert.True(drone.IsKnocked);
        Assert.Same(stored, result);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GameService_GetOrCreateCurrentGameAsync_WhenTheStoredDoneFlagIsStale_RecalculatesItAndPersistsTheCorrection()
    {
        // ARRANGE
        var stored = new Game
        {
            IsGameDone = false,
            Wasps = new List<Wasp> { Knocked(new Queen(), Now.AddSeconds(30)), new Drone() }
        };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.GetOrCreateCurrentGameAsync();

        // ASSERT
        Assert.True(result.IsGameDone);
        _repository.Verify(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ---------------------------------------------------------------------
    // GameService.ResetGameAsync
    // ---------------------------------------------------------------------

    [Fact]
    public async Task GameService_ResetGameAsync_WhenCalled_ReplacesTheCurrentGameWithAFullHealthySwarm()
    {
        // ARRANGE
        Game? persisted = null;
        _repository.Setup(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .Callback<Game, CancellationToken>((game, _) => persisted = game)
            .ReturnsAsync((Game game, CancellationToken _) => game);
        var sut = CreateSut();

        // ACT
        var result = await sut.ResetGameAsync();

        // ASSERT
        Assert.NotNull(persisted);
        Assert.Same(persisted, result);
        Assert.Equal(13, result.Wasps.Count);
        Assert.True(result.IsActive);
        Assert.False(result.IsGameDone);
        Assert.All(result.Wasps, wasp => Assert.Equal(wasp.MaxEnergy, wasp.Energy));
    }

    [Fact]
    public async Task GameService_ResetGameAsync_WhenCalled_ReturnsTheGameTheRepositoryPersistedRatherThanTheLocalDraft()
    {
        // ARRANGE
        var persistedByRepository = new Game { Id = 42, Wasps = new List<Wasp> { new Queen() } };
        _repository.Setup(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistedByRepository);
        var sut = CreateSut();

        // ACT
        var result = await sut.ResetGameAsync();

        // ASSERT
        Assert.Same(persistedByRepository, result);
        Assert.Equal(42, result.Id);
    }

    // ---------------------------------------------------------------------
    // GameService.ZapAsync
    // ---------------------------------------------------------------------

    [Fact]
    public async Task GameService_ZapAsync_WhenNoGameExistsYet_CreatesZapsAndSavesTheGameInASingleWrite()
    {
        // ARRANGE
        Game? persisted = null;
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);
        _repository.Setup(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .Callback<Game, CancellationToken>((game, _) => persisted = game)
            .ReturnsAsync((Game game, CancellationToken _) => game);
        var sut = CreateSut();

        // ACT
        var result = await sut.ZapAsync();

        // ASSERT
        Assert.NotNull(persisted);
        Assert.Same(persisted, result.Game);
        Assert.Single(persisted.Wasps, w => w.Energy < w.MaxEnergy);
        _repository.Verify(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GameService_ZapAsync_WhenAGameIsInPlay_DamagesTheTargetedWaspAndPersistsTheUpdate()
    {
        // ARRANGE
        var queen = new Queen { Id = 7 };
        var stored = new Game { Id = 1, Wasps = new List<Wasp> { queen, new Drone { Id = 8 } } };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.ZapAsync();

        // ASSERT
        Assert.Same(stored, result.Game);
        Assert.Equal(7, result.HitWaspId);
        Assert.Equal(100 - GameRules.ZapDamage, queen.Energy);
        _repository.Verify(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GameService_ZapAsync_WhenTheZapDrainsTheQueen_EndsTheGameAndPersistsTheFinishedState()
    {
        // ARRANGE
        var queen = new Queen { Id = 7, Energy = GameRules.ZapDamage };
        var stored = new Game { Wasps = new List<Wasp> { queen, new Drone { Id = 8 } } };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.ZapAsync();

        // ASSERT
        Assert.True(result.Game.IsGameDone);
        Assert.Equal(7, result.HitWaspId);
        Assert.True(queen.IsKnocked);
        _repository.Verify(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GameService_ZapAsync_WhenTheGameIsAlreadyOver_HitsNobodyAndSkipsTheWriteEntirely()
    {
        // ARRANGE
        var stored = new Game
        {
            IsGameDone = true,
            Wasps = new List<Wasp> { Knocked(new Queen { Id = 7 }, Now.AddSeconds(-100)), new Drone { Id = 8 } }
        };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        var result = await sut.ZapAsync();

        // ASSERT
        Assert.Null(result.HitWaspId);
        Assert.Same(stored, result.Game);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.ReplaceCurrentAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GameService_ZapAsync_WhenGivenACancellationToken_PassesThatSameTokenToEveryRepositoryCall()
    {
        // ARRANGE
        using var cts = new CancellationTokenSource();
        var stored = GameLogic.Create();
        _repository.Setup(r => r.GetCurrentAsync(cts.Token)).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, cts.Token)).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        await sut.ZapAsync(cts.Token);

        // ASSERT
        _repository.Verify(r => r.GetCurrentAsync(cts.Token), Times.Once);
        _repository.Verify(r => r.UpdateAsync(stored, cts.Token), Times.Once);
    }

    [Fact]
    public async Task GameService_ZapAsync_WhenZappedRepeatedlyAtTheQueen_EndsTheGameAfterTenZapsAndStopsHittingAfterwards()
    {
        // ARRANGE
        var stored = new Game { Wasps = new List<Wasp> { new Queen { Id = 7 }, new Drone { Id = 8 } } };
        _repository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _repository.Setup(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        var sut = CreateSut();

        // ACT
        for (var zap = 0; zap < 10; zap++)
        {
            await sut.ZapAsync();
        }

        var zapAfterTheGameEnded = await sut.ZapAsync();

        // ASSERT
        Assert.True(stored.IsGameDone); // 100 energy / 11 damage, rounded up.
        Assert.Null(zapAfterTheGameEnded.HitWaspId);
        _repository.Verify(r => r.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Exactly(10));
    }
}
