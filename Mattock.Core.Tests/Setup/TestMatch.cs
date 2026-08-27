using System.Text;
using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Tests.Setup.Asserts;

namespace Mattock.Core.Tests.Setup;

public class TestSession(
    MatchConfig config,
    PlayerSetup[] setups,
    Mechanics mechanics,
    string[] setupScripts
) : Session(
    config, 
    setups, 
    mechanics,
    setupScripts
)
{
}

public class TestSessionWrapper
{
    public delegate void PreLaunchAction(Session match);

    public MatchConfig Config { get; }
    public Exception? Exception { get; private set; }
    public TestPlayerController[] Players { get; }
    public Mechanics Mechanics { get; }
    public List<PreLaunchAction> PreLaunchActions { get; }

    public TestSession? Session { get; private set; }

    public TestSessionWrapper(MatchConfig config, TestPlayerControllerBuilder[] players)
    {
        Config = config;

        Session = null;
        Exception = null;
        Players = [.. players.Select(p => p.Build(this))];
        Mechanics = new();
        PreLaunchActions = [];
    }

    public Match GetMatch()
        => Session!.Match;

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
        var coreScript = CoreLoader.Load("../../../../core");

        Session = new TestSession(
            Config,
            [ .. Players.Select(p => p.GetPlayerSetup() )],
            Mechanics,
            coreScript
        );

        foreach (var act in PreLaunchActions)
            act(Session);

        try
        {
            await Session.Run();
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }

    public TestSessionWrapper Assert(Action<MatchAsserts> action)
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