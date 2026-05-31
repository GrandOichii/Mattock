using System.Runtime.ExceptionServices;

namespace Mattock.Core.Tests.Setup.Asserts;

public class MatchAsserts(TestMatchWrapper match)
{
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
        m.ActivePlayerIdx.ShouldBe(playerIdx);
        return this;
    }

    public MatchAsserts AssertPlayer(int playerIdx, Action<PlayerAsserts> action)
    {
        action(new(match.Match!.Players[playerIdx]));
        return this;
    }
}