using Application.Utils;
using Domain;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Utils;

public class GameLogicTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private static Random RandomPicking(int index)
    {
        var random = new Mock<Random>();
        random.Setup(r => r.Next(It.IsAny<int>())).Returns(index);

        return random.Object;
    }

    private static Wasp Knocked(Wasp wasp, DateTimeOffset? knockedUntil)
    {
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = knockedUntil;

        return wasp;
    }

    [Fact]
    public void GameLogic_Create_WhenStartingANewGame_BuildsAnActiveUnfinishedSwarmOfOneQueenFiveDronesAndSevenWorkers()
    {
        // ARRANGE / ACT
        var game = GameLogic.Create();

        // ASSERT
        Assert.True(game.IsActive);
        Assert.False(game.IsGameDone);
        Assert.Equal(13, game.Wasps.Count);
        Assert.Single(game.Wasps.OfType<Queen>());
        Assert.Equal(5, game.Wasps.OfType<Drone>().Count());
        Assert.Equal(7, game.Wasps.OfType<Worker>().Count());
    }

    [Fact]
    public void GameLogic_Create_WhenStartingANewGame_GivesEveryWaspFullEnergyAndNoKnockoutState()
    {
        // ARRANGE / ACT
        var game = GameLogic.Create();

        // ASSERT
        Assert.All(game.Wasps, wasp =>
        {
            Assert.Equal(wasp.MaxEnergy, wasp.Energy);
            Assert.False(wasp.IsKnocked);
            Assert.Null(wasp.KnockedUntil);
        });
    }

    // ---------------------------------------------------------------------
    // GameLogic.RecoverWasps
    // ---------------------------------------------------------------------

    [Fact]
    public void GameLogic_RecoverWasps_WhenSeveralWaspsAreDue_RecoversEveryDueWaspAndLeavesTheRestKnockedOut()
    {
        // ARRANGE
        var firstDue = Knocked(new Drone(), Now.AddSeconds(-1));
        var secondDue = Knocked(new Worker(), Now.AddSeconds(-1));
        var notDue = Knocked(new Worker(), Now.AddSeconds(30));
        var game = new Game { Wasps = new List<Wasp> { new Queen(), firstDue, secondDue, notDue } };

        // ACT
        var recovered = game.RecoverWasps(Now);

        // ASSERT
        Assert.True(recovered);
        Assert.False(firstDue.IsKnocked);
        Assert.False(secondDue.IsKnocked);
        Assert.True(notDue.IsKnocked);
    }

    [Fact]
    public void GameLogic_RecoverWasps_WhenNoWaspIsDue_ReportsNoChange()
    {
        // ARRANGE
        var game = GameLogic.Create();

        // ACT
        var recovered = game.RecoverWasps(Now);

        // ASSERT
        Assert.False(recovered);
    }

    [Fact]
    public void GameLogic_RecoverWasps_WhenTheGameIsAlreadyDone_FreezesTheSwarmAndSkipsRecoveryEntirely()
    {
        // ARRANGE
        var queen = Knocked(new Queen(), Now.AddSeconds(-100));
        var game = new Game { IsGameDone = true, Wasps = new List<Wasp> { queen, new Drone() } };

        // ACT
        var recovered = game.RecoverWasps(Now);

        // ASSERT
        Assert.False(recovered);
        Assert.True(queen.IsKnocked);
    }

    // ---------------------------------------------------------------------
    // GameLogic.Refresh
    // ---------------------------------------------------------------------

    [Fact]
    public void GameLogic_Refresh_WhenNothingHasChangedSinceTheLastLook_ReportsNoChangeSoNothingIsPersisted()
    {
        // ARRANGE
        var game = GameLogic.Create();

        // ACT
        var changed = game.Refresh(Now);

        // ASSERT
        Assert.False(changed);
        Assert.False(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_Refresh_WhenAKnockedWaspBecomesDue_RecoversItAndReportsTheChange()
    {
        // ARRANGE
        var drone = Knocked(new Drone(), Now.AddSeconds(-1));
        var game = new Game { Wasps = new List<Wasp> { new Queen(), drone } };

        // ACT
        var changed = game.Refresh(Now);

        // ASSERT
        Assert.True(changed);
        Assert.False(drone.IsKnocked);
        Assert.False(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_Refresh_WhenTheStoredDoneFlagDisagreesWithTheSwarm_CorrectsTheFlagAndReportsTheChange()
    {
        // ARRANGE
        var game = new Game
        {
            IsGameDone = false,
            Wasps = new List<Wasp> { Knocked(new Queen(), Now.AddSeconds(60)), new Drone() }
        };

        // ACT
        var changed = game.Refresh(Now);

        // ASSERT
        Assert.True(changed);
        Assert.True(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_Refresh_WhenTheGameIsFinished_LeavesItFinishedAndReportsNoChange()
    {
        // ARRANGE
        var game = new Game
        {
            IsGameDone = true,
            Wasps = new List<Wasp> { Knocked(new Queen(), Now.AddSeconds(-100)), new Drone() }
        };

        // ACT
        var changed = game.Refresh(Now);

        // ASSERT
        Assert.False(changed);
        Assert.True(game.IsGameDone);
    }

    // ---------------------------------------------------------------------
    // GameLogic.Zap
    // ---------------------------------------------------------------------

    [Fact]
    public void GameLogic_Zap_WhenTheSwarmIsHealthy_DealsTheConfiguredZapDamageToTheChosenWasp()
    {
        // ARRANGE
        var game = GameLogic.Create();
        var expectedTarget = game.Wasps.First();

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Same(expectedTarget, outcome.Hit);
        Assert.True(outcome.StateChanged);
        Assert.Equal(expectedTarget.MaxEnergy - GameRules.ZapDamage, expectedTarget.Energy);
    }

    [Fact]
    public void GameLogic_Zap_WhenSomeWaspsAreKnockedOut_OnlyOffersAndHitsTheWaspsStillStanding()
    {
        // ARRANGE
        var standing = new Worker();
        var game = new Game
        {
            Wasps = new List<Wasp>
            {
                new Queen(),
                Knocked(new Drone(), Now.AddSeconds(60)),
                Knocked(new Worker(), Now.AddSeconds(60)),
                standing
            }
        };
        var random = new Mock<Random>();
        random.Setup(r => r.Next(It.IsAny<int>())).Returns(1);

        // ACT
        var outcome = game.Zap(random.Object, Now);

        // ASSERT
        random.Verify(r => r.Next(2), Times.Once); // Two of the four wasps were candidates.
        Assert.Same(standing, outcome.Hit);
        Assert.Equal(75 - GameRules.ZapDamage, standing.Energy);
    }

    [Fact]
    public void GameLogic_Zap_WhenTheHitDrainsTheLastOfAWaspsEnergy_KnocksThatWaspOutForTheConfiguredDuration()
    {
        // ARRANGE
        var drone = new Drone();
        drone.Energy = GameRules.ZapDamage;
        var game = new Game { Wasps = new List<Wasp> { drone, new Queen() } };

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Same(drone, outcome.Hit);
        Assert.True(drone.IsKnocked);
        Assert.Equal(Now.Add(GameRules.KnockoutDuration), drone.KnockedUntil);
    }

    [Fact]
    public void GameLogic_Zap_WhenTheQueenIsKnockedOut_EndsTheGameEvenThoughOtherWaspsAreStillStanding()
    {
        // ARRANGE
        var queen = new Queen();
        queen.Energy = GameRules.ZapDamage;
        var game = new Game { Wasps = new List<Wasp> { queen, new Drone(), new Worker() } };

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Same(queen, outcome.Hit);
        Assert.True(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_Zap_WhenTheLastStandingWaspIsKnockedOut_EndsTheGame()
    {
        // ARRANGE
        var worker = new Worker();
        worker.Energy = GameRules.ZapDamage;
        var game = new Game
        {
            Wasps = new List<Wasp> { Knocked(new Queen(), Now.AddSeconds(60)), worker }
        };

        // ACT
        game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.True(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_Zap_WhenTheGameIsAlreadyDone_HitsNobodyAndReportsNoChange()
    {
        // ARRANGE
        var drone = new Drone();
        var game = new Game
        {
            IsGameDone = true,
            Wasps = new List<Wasp> { Knocked(new Queen(), Now.AddSeconds(-100)), drone }
        };

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Null(outcome.Hit);
        Assert.False(outcome.StateChanged);
        Assert.Equal(60, drone.Energy);
    }

    [Fact]
    public void GameLogic_Zap_WhenEveryWaspIsKnockedOutButTheDoneFlagIsStale_HitsNobodyInsteadOfThrowing()
    {
        // ARRANGE
        var game = new Game
        {
            IsGameDone = false,
            Wasps = new List<Wasp>
            {
                Knocked(new Queen(), Now.AddSeconds(60)),
                Knocked(new Drone(), Now.AddSeconds(60))
            }
        };

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Null(outcome.Hit);
        Assert.False(outcome.StateChanged);
    }

    [Fact]
    public void GameLogic_Zap_WhenAKnockedWaspHasBecomeDue_RecoversItFirstAndTreatsItAsAValidTargetAgain()
    {
        // ARRANGE
        var drone = Knocked(new Drone(), Now.AddSeconds(-1));
        var game = new Game { Wasps = new List<Wasp> { drone, new Queen() } };

        // ACT
        var outcome = game.Zap(RandomPicking(0), Now);

        // ASSERT
        Assert.Same(drone, outcome.Hit);
        Assert.False(drone.IsKnocked);
        Assert.Equal(60 - GameRules.ZapDamage, drone.Energy);
    }

    // ---------------------------------------------------------------------
    // GameLogic.UpdateGameDone
    // ---------------------------------------------------------------------

    [Fact]
    public void GameLogic_UpdateGameDone_WhenTheQueenIsKnockedOut_MarksTheGameAsDone()
    {
        // ARRANGE
        var game = new Game
        {
            Wasps = new List<Wasp> { Knocked(new Queen(), Now), new Drone(), new Worker() }
        };

        // ACT
        game.UpdateGameDone();

        // ASSERT
        Assert.True(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_UpdateGameDone_WhenEveryWaspIsKnockedOut_MarksTheGameAsDone()
    {
        // ARRANGE
        var game = new Game
        {
            Wasps = new List<Wasp> { Knocked(new Drone(), Now), Knocked(new Worker(), Now) }
        };

        // ACT
        game.UpdateGameDone();

        // ASSERT
        Assert.True(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_UpdateGameDone_WhenTheQueenIsStandingAndSomeWaspsRemain_LeavesTheGameInPlay()
    {
        // ARRANGE
        var game = new Game
        {
            IsGameDone = true, // flag must be cleared, not just left alone.
            Wasps = new List<Wasp> { new Queen(), Knocked(new Drone(), Now), new Worker() }
        };

        // ACT
        game.UpdateGameDone();

        // ASSERT
        Assert.False(game.IsGameDone);
    }

    [Fact]
    public void GameLogic_UpdateGameDone_WhenTheGameHasNoWasps_TreatsItAsDoneBecauseNothingIsLeftStanding()
    {
        // ARRANGE
        var game = new Game { Wasps = new List<Wasp>() };

        // ACT
        game.UpdateGameDone();

        // ASSERT
        Assert.True(game.IsGameDone);
    }
}
