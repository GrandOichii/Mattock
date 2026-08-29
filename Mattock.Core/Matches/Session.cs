using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches;

public class Session
{
    public MatchConfig Config { get; }
    public PlayerSetup[] PlayerSetups { get; }
    public Mechanics Mechanics { get; }
    public string[] SetupScripts { get; }
    public SnapshotsManager Snapshots { get; }

    public Match Match { get; private set; }


    public Session(
        MatchConfig config,
        PlayerSetup[] playerSetups,
        Mechanics mechanics,
        string[] setupScripts
    )
    {
        Config = config;
        PlayerSetups = playerSetups;
        Mechanics = mechanics;
        SetupScripts = setupScripts;
        Snapshots = new(this);

        Match = CreateMatch();
    }

    public Match CreateMatch()
        => new(this, Config, PlayerSetups, Mechanics, SetupScripts);

    // public MatchSnapshot GetMatchSnapshot()
    //     => Match.GetSnapshot();

    public async Task Run()
    {
        // TODO call setup for player controllers

        await Match.Setup();
        while (true)
        {
            var request = await Match.Run();
            if (request is null) break;

            var snap = Snapshots.GetAndClear(request.RequestedSnapshotId);

            Match = CreateMatch();
            await Match.LoadSnapshot(snap);
        }
    }
    
}