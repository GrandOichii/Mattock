using Mattock.Core.Matches.Players.Controllers;

namespace Mattock.Core.Matches.Snapshots;

public class Snapshot(
    int id,
    PlayerResponsesRecord[] playerRecords,
    string description,
    Match match
)
{
    public int Id { get; } = id;
    // public MatchSnapshot Snap { get; } = snapshot; 
    public PlayerResponsesRecord[] PlayerRecords { get; } = playerRecords;

    public Metadata Meta { get; } = new(
        description,
        match.TurnManager.TurnCounter
    );

    public record Metadata
    (
        string Description,
        int TurnCounter
    );
}