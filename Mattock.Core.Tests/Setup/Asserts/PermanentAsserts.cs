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
}