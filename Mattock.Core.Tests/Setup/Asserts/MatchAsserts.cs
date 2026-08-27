using System.Runtime.ExceptionServices;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Setup.Asserts;

public class MatchAsserts(TestSessionWrapper match)
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
        bool checkPlayersChoices = true,
        bool checkPermanentsChoices = true,
        bool checkStringChoices = true,
        bool checkCardChoices = true,
        bool checkCostCollectionChoices = true,
        bool checkStoredManaChoices = true,
        bool checkAttackDeclarationsChoices = true,
        bool checkBlockDeclarationsChoices = true
    )
    {
        foreach (var player in match.Players)
        {
            player.AssertNoChoicesLeft(
                checkCommandChoices,
                checkPlayersChoices,
                checkPermanentsChoices,
                checkStringChoices,
                checkCardChoices,
                checkCostCollectionChoices,
                checkStoredManaChoices,
                checkAttackDeclarationsChoices,
                checkBlockDeclarationsChoices
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

    public MatchAsserts TurnNumber(int v)
    {
        match.GetMatch().TurnManager.TurnCounter.ShouldBe(v);
        return this;
    }

    public MatchAsserts ActivePlayerIs(int playerIdx)
    {
        var m = match.GetMatch();
        m.TurnManager.ActivePlayerIdx.ShouldBe(playerIdx);
        return this;
    }

    public MatchAsserts AssertPlayer(int playerIdx, Action<PlayerAsserts> action)
    {
        action(new(match.GetMatch().Players[playerIdx]));
        return this;
    }

    public MatchAsserts AssertBattlefield(Action<BattlefieldAsserts> action)
    {
        action(new(match.GetMatch().Battlefield));
        return this;
    }

    public MatchAsserts MatchPhases(params PhaseType[] phases)
    {
        var turn = match.GetMatch().TurnManager;
        turn.Phases.Count.ShouldBe(phases.Length);

        for (int i = 0; i < phases.Length; ++i)
        {
            turn.Phases[i].Type.ShouldBe(phases[i]);
        }
        return this;
    }

    public MatchAsserts CurrentPhase(PhaseType type)
    {
        match.GetMatch().TurnManager.GetCurrentPhase().Type.ShouldBe(type);
        return this;
    }

    public MatchAsserts CurrentStep(StepType type)
    {
        match.GetMatch().TurnManager.GetCurrentPhase().GetCurrentStep().ShouldNotBeNull();
        match.GetMatch().TurnManager.GetCurrentPhase().GetCurrentStep()!.Type.ShouldBe(type);
        return this;
    }

    public MatchAsserts NoWinnersDecided()
    {
        match.GetMatch().ShouldHalt().ShouldBeFalse();
        return this;
    }

    public MatchAsserts WinningTeams(int[] teams)
    {
        match.GetMatch().GetWinningTeams().ShouldBeEquivalentTo(teams);
        return this;
    }

    public MatchAsserts AssertStack(Action<StackAsserts> action)
    {
        action(new(match.GetMatch().Stack));
        return this;
    }

    public MatchAsserts AssertSnapshots(Action<SnapshotsAsserts> action)
    {
        action(new(match.Session!.Snapshots));
        return this;
    }
}