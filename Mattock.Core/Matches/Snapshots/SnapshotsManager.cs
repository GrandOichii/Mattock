namespace Mattock.Core.Matches.Snapshots;

public class SnapshotsManager(
    Session session
)
{
    public List<Snapshot> Snapshots { get; } = [];

    public Snapshot CreateSnapshot(string id)
    {
        if (Snapshots.Any(s => s.Id == id))
            throw new CodeErrorException($"Snapshot with Id = {id} already exists");

        Snapshot snap = new(
            id,
            // session.GetMatchSnapshot(),
            [.. session.Match.Players.Select(p => p.GetRecord())]
        );
        Snapshots.Add(snap);

        while (Snapshots.Count > session.Config.SnapshotMemory)
            Snapshots.Remove(Snapshots[0]);
            
        return snap;
    }

    public Snapshot? Get(string id)
    {
        return Snapshots.SingleOrDefault(s => s.Id == id);
    }
}