using System.Runtime.ExceptionServices;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Setup.Asserts;

public class MatchAsserts(TestMatchWrapper match)
{
    public MatchAsserts DidntCrash()
    {
        if (match.Exception is not null)
        {
            ExceptionDispatchInfo.Capture(match.Exception).Throw();
        }

        return this;
    }
    
    public MatchAsserts NoChoicesLeft(
        bool checkCommandChoices = true,
        bool checkPlayerChoices = true,
        bool checkStringChoices = true,
        bool checkCardChoices = true,
        bool checkCostCollectionChoices = true,
        bool checkStoredManaChoices = true
    )
    {
        foreach (var player in match.Players)
        {
            player.AssertNoChoicesLeft(
                checkCommandChoices,
                checkPlayerChoices,
                checkStringChoices,
                checkCardChoices,
                checkCostCollectionChoices,
                checkStoredManaChoices
            );        
        }
        return this;
    }

    public MatchAsserts CrashedIntentially()
    {
        match.Exception.ShouldNotBeNull();
        if (match.Exception.GetType() != typeof(IntentionalCrashException))
        {
            ExceptionDispatchInfo.Capture(match.Exception).Throw();
        }

        return this;
    }

    public MatchAsserts ActivePlayerIs(int playerIdx)
    {
        var m = match.Match!;
        m.TurnManager.ActivePlayerIdx.ShouldBe(playerIdx);
        return this;
    }

    public MatchAsserts AssertPlayer(int playerIdx, Action<PlayerAsserts> action)
    {
        action(new(match.Match!.Players[playerIdx]));
        return this;
    }

    public MatchAsserts AssertBattlefield(Action<BattlefieldAsserts> action)
    {
        action(new(match.Match!.Battlefield));
        return this;
    }

    public MatchAsserts MatchPhases(params PhaseType[] phases)
    {
        var turn = match.Match!.TurnManager;
        turn.Phases.Count.ShouldBe(phases.Length);

        for (int i = 0; i < phases.Length; ++i)
        {
            turn.Phases[i].Type.ShouldBe(phases[i]);
        }
        return this;
    }

    public MatchAsserts CurrentPhase(PhaseType type)
    {
        match.Match!.TurnManager.GetCurrentPhase().Type.ShouldBe(type);
        return this;
    }

    public MatchAsserts CurrentStep(StepType type)
    {
        match.Match!.TurnManager.GetCurrentPhase().GetCurrentStep().ShouldNotBeNull();
        match.Match!.TurnManager.GetCurrentPhase().GetCurrentStep()!.Type.ShouldBe(type);
        return this;
    }

    public MatchAsserts AssertStack(Action<StackAsserts> action)
    {
        action(new(match.Match!.Stack));
        return this;
    }
}