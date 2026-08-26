using Mattock.Core.Matches.Players.Controllers;

namespace Mattock.Core.Matches.Snapshots;

public class Snapshot(
    string id,
    PlayerResponsesRecord[] playerRecords
    // Match.Snapshot snapshot
)
{
    public string Id { get; } = id;
    // public Match.Snapshot Snap { get; } = snapshot; 
    public PlayerResponsesRecord[] PlayerRecords { get; } = playerRecords;

}