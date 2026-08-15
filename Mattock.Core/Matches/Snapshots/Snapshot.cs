namespace Mattock.Core.Matches.Snapshots;

public class Snapshot(
    string id,
    Match.Snapshot snapshot
)
{
    public string Id { get; } = id;
    public Match.Snapshot Snap { get; } = snapshot; 
}