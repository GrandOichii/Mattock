using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Tests.Setup.Asserts;

namespace Mattock.Core.Tests.Setup;

public class TestMatch : Match
{
    public TestMatch(
        MatchConfig config,
        PlayerSetup[] setups,
        Mechanics mechanics
    ) : base(
        config, 
        setups, 
        mechanics
    )
    {
    }
}

public class TestMatchWrapper
{
    public MatchConfig Config { get; }
    public Exception? Exception { get; private set; }
    public TestPlayerController[] Players { get; }
    public Mechanics Mechanics { get; }

    public TestMatch? Match { get; private set; }

    public TestMatchWrapper(MatchConfig config, TestPlayerControllerBuilder[] players)
    {
        Config = config;

        Match = null;
        Exception = null;
        Players = [.. players.Select(p => p.Build(this))];
        Mechanics = new();
    }

    public void SetMulligan(IMulliganRule mulligan)
    {
        Mechanics.Mulligan = mulligan;
    }

    public void RemoveMulligans()
    {
        Mechanics.Mulligan = null;
    }

    public async Task Run()
    {
        // var core = File.ReadAllText("../../../../core.lua");

        Match = new TestMatch(
            Config,
            [ .. Players.Select(p => p.GetPlayerSetup() )],
            Mechanics
        );

        try
        {
            await Match.Run();
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }

    public TestMatchWrapper Assert(Action<MatchAsserts> action)
    {
        action(new(this));
        return this;
    }

    // public TestMatchWrapper AssertPlayer(int playerIdx, Action<PlayerAsserts> action)
    // {
    //     Match.ShouldNotBeNull();
    //     action(new(Match.Players[playerIdx]));
    //     return this;
    // }
}