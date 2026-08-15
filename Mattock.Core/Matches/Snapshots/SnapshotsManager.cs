namespace Mattock.Core.Matches.Snapshots;

public class SnapshotsManager(
    Match match
)
{
    public List<Snapshot> Snapshots { get; } = [];

    public Snapshot CreateSnapshot(string id)
    {
        if (Snapshots.Any(s => s.Id == id))
            throw new Exception($"Snapshot with Id = {id} already exists"); // TODO type

        Snapshot snap = new(id, match.GetSnapshot());
        Snapshots.Add(snap);
        return snap;
    }
}