using Mattock.Core.Matches.Players.Controllers;

namespace Mattock.Core.Matches.Snapshots;

public class Snapshot(
    string id,
    MatchSnapshot snapshot,
    PlayerResponsesRecord[] playerRecords
)
{
    public string Id { get; } = id;
    public MatchSnapshot Snap { get; } = snapshot; 
    public PlayerResponsesRecord[] PlayerRecords { get; } = playerRecords;

}