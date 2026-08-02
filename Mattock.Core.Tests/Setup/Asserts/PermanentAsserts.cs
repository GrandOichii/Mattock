using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Tests.Setup.Asserts;

public class PermanentAsserts(Permanent permanent)
{
    public PermanentAsserts IsLand()
    {
        return IsOfType(CardTypes.Land);
    }

    public PermanentAsserts IsOfType(string type)
    {
        permanent.HasType(type).ShouldBeTrue(
            $"Expected permanent {permanent.GetDisplayName()} to have type {type}, but it didn't"
        );
        return this;
    }

    public PermanentAsserts ControlledBy(int playerIdx)
    {
        permanent.IsControlledBy(playerIdx).ShouldBeTrue();
        return this;
    }

    public PermanentAsserts IsUntapped()
    {
        permanent.IsUntapped().ShouldBeTrue();
        return this;
    }

    public PermanentAsserts IsTapped()
    {
        permanent.IsTapped().ShouldBeTrue();
        return this;
    }

    public PermanentAsserts CheckTapped(bool expected)
    {
        permanent.IsTapped().ShouldBe(expected);
        return this;
    }

    public PermanentAsserts IsNotAttacking()
    {
        permanent.IsAttacking().ShouldBeFalse();
        return this;
    }

    public PermanentAsserts IsAttackingPlayer(int idx)
    {
        var player = permanent.Match.Players[idx];
        permanent.CombatState.ShouldNotBeNull();
        permanent.CombatState.AttackTarget.GetTarget().ShouldBe(player);
        return this;
    }

    public PermanentAsserts IsBlocking()
    {
        var match = permanent.Match;
        match.Battlefield.GetPermanents().Any(p => 
            p.CombatState is not null &&
            p.CombatState.BlockedBy.Contains(permanent)
        ).ShouldBeTrue();

        return this;
    }

    public PermanentAsserts IsNotBlocking()
    {
        var match = permanent.Match;
        match.Battlefield.GetPermanents().Any(p => 
            p.CombatState is not null &&
            p.CombatState.BlockedBy.Contains(permanent)
        ).ShouldBeFalse();

        return this;
    }

    public PermanentAsserts IsBlocking(string attackerName)
    {
        var match = permanent.Match;
        match.Battlefield.GetPermanents().Any(p => 
            p.HasName(attackerName) &&
            p.CombatState is not null &&
            p.CombatState.BlockedBy.Contains(permanent)
        ).ShouldBeTrue();

        return this;
    }

    public PermanentAsserts IsNotBlocking(string attackerName)
    {
        var match = permanent.Match;
        match.Battlefield.GetPermanents().Any(p => 
            p.HasName(attackerName) &&
            p.CombatState is not null &&
            p.CombatState.BlockedBy.Contains(permanent)
        ).ShouldBeFalse();

        return this;
    }

    public PermanentAsserts HasMarkedDamage(int amount)
    {
        permanent.MarkedDamage.ShouldBe(amount);
        return this;
    }

    public PermanentAsserts HasNoMarkedDamage()
    {
        permanent.MarkedDamage.ShouldBe(0);
        return this;
    }
}