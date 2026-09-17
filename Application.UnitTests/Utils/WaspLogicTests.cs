using Application.Utils;
using Domain.Entities;

namespace Application.UnitTests.Utils;

public class WaspLogicTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan KnockoutDuration = TimeSpan.FromSeconds(40);

    // ---------------------------------------------------------------------
    // WaspLogic.TakeDamage
    // ---------------------------------------------------------------------

    [Fact]
    public void WaspLogic_TakeDamage_WhenDamageIsLessThanEnergy_ReducesEnergyAndLeavesTheWaspStanding()
    {
        // ARRANGE
        var wasp = new Queen();

        // ACT
        var changed = wasp.TakeDamage(11, KnockoutDuration, Now);

        // ASSERT
        Assert.True(changed);
        Assert.Equal(89, wasp.Energy);
        Assert.False(wasp.IsKnocked);
        Assert.Null(wasp.KnockedUntil);
    }

    [Fact]
    public void WaspLogic_TakeDamage_WhenDamageExactlyMatchesRemainingEnergy_KnocksTheWaspOutAndSetsRecoveryDeadline()
    {
        // ARRANGE
        var wasp = new Drone();
        wasp.Energy = 11;

        // ACT
        var changed = wasp.TakeDamage(11, KnockoutDuration, Now);

        // ASSERT
        Assert.True(changed);
        Assert.Equal(0, wasp.Energy);
        Assert.True(wasp.IsKnocked);
        Assert.Equal(Now.Add(KnockoutDuration), wasp.KnockedUntil);
    }

    [Fact]
    public void WaspLogic_TakeDamage_WhenDamageExceedsRemainingEnergy_ClampsEnergyAtZeroInsteadOfGoingNegative()
    {
        // ARRANGE
        var wasp = new Worker();
        wasp.Energy = 3;

        // ACT
        var changed = wasp.TakeDamage(500, KnockoutDuration, Now);

        // ASSERT
        Assert.True(changed);
        Assert.Equal(0, wasp.Energy);
        Assert.True(wasp.IsKnocked);
    }

    [Fact]
    public void WaspLogic_TakeDamage_WhenTheWaspIsAlreadyKnockedOut_IgnoresTheDamageAndReportsNoChange()
    {
        // ARRANGE
        var wasp = new Drone();
        var knockedUntil = Now.AddSeconds(10);
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = knockedUntil;

        // ACT
        var changed = wasp.TakeDamage(11, KnockoutDuration, Now);

        // ASSERT
        Assert.False(changed);
        Assert.Equal(0, wasp.Energy);
        Assert.Equal(knockedUntil, wasp.KnockedUntil);
    }

    [Fact]
    public void WaspLogic_TakeDamage_WhenDamageIsZero_LeavesEnergyUntouchedAndReportsNoChange()
    {
        // ARRANGE
        var wasp = new Worker();

        // ACT
        var changed = wasp.TakeDamage(0, KnockoutDuration, Now);

        // ASSERT
        Assert.False(changed);
        Assert.Equal(75, wasp.Energy);
        Assert.False(wasp.IsKnocked);
    }

    // ---------------------------------------------------------------------
    // WaspLogic.Recover
    // ---------------------------------------------------------------------

    [Fact]
    public void WaspLogic_Recover_WhenTheWaspIsKnockedOut_RestoresFullEnergyAndClearsTheKnockoutState()
    {
        // ARRANGE
        var wasp = new Queen();
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = Now;

        // ACT
        wasp.Recover();

        // ASSERT
        Assert.Equal(wasp.MaxEnergy, wasp.Energy);
        Assert.Equal(100, wasp.Energy);
        Assert.False(wasp.IsKnocked);
        Assert.Null(wasp.KnockedUntil);
    }

    // ---------------------------------------------------------------------
    // WaspLogic.RecoverIfDue
    // ---------------------------------------------------------------------

    [Fact]
    public void WaspLogic_RecoverIfDue_WhenTheKnockoutDeadlineHasPassed_RecoversTheWaspAndReportsTheChange()
    {
        // ARRANGE
        var wasp = new Drone();
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = Now.AddSeconds(-1);

        // ACT
        var recovered = wasp.RecoverIfDue(Now);

        // ASSERT
        Assert.True(recovered);
        Assert.Equal(60, wasp.Energy);
        Assert.False(wasp.IsKnocked);
        Assert.Null(wasp.KnockedUntil);
    }

    [Fact]
    public void WaspLogic_RecoverIfDue_WhenTheKnockoutDeadlineIsExactlyNow_TreatsTheWaspAsDueAndRecoversIt()
    {
        // ARRANGE
        var wasp = new Worker();
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = Now;

        // ACT
        var recovered = wasp.RecoverIfDue(Now);

        // ASSERT
        Assert.True(recovered);
        Assert.False(wasp.IsKnocked);
        Assert.Equal(75, wasp.Energy);
    }

    [Fact]
    public void WaspLogic_RecoverIfDue_WhenTheKnockoutDeadlineIsStillInTheFuture_LeavesTheWaspKnockedOut()
    {
        // ARRANGE
        var wasp = new Queen();
        var knockedUntil = Now.AddSeconds(1);
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = knockedUntil;

        // ACT
        var recovered = wasp.RecoverIfDue(Now);

        // ASSERT
        Assert.False(recovered);
        Assert.True(wasp.IsKnocked);
        Assert.Equal(0, wasp.Energy);
        Assert.Equal(knockedUntil, wasp.KnockedUntil);
    }

    [Fact]
    public void WaspLogic_RecoverIfDue_WhenTheWaspIsNotKnockedOut_DoesNothingEvenThoughItIsDamaged()
    {
        // ARRANGE
        var wasp = new Drone();
        wasp.Energy = 12;

        // ACT
        var recovered = wasp.RecoverIfDue(Now);

        // ASSERT
        Assert.False(recovered);
        Assert.Equal(12, wasp.Energy);
    }

    [Fact]
    public void WaspLogic_RecoverIfDue_WhenTheWaspIsKnockedOutWithNoDeadline_LeavesItKnockedOutRatherThanRecoveringForever()
    {
        // ARRANGE
        var wasp = new Worker();
        wasp.Energy = 0;
        wasp.IsKnocked = true;
        wasp.KnockedUntil = null;

        // ACT
        var recovered = wasp.RecoverIfDue(Now);

        // ASSERT
        Assert.False(recovered);
        Assert.True(wasp.IsKnocked);
        Assert.Equal(0, wasp.Energy);
    }
}
