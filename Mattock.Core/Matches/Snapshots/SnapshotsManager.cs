namespace Mattock.Core.Matches.Snapshots;

public class SnapshotsManager(
    Session session
)
{
    private int _lastSnapshotId = 0;
    public LinkedList<Snapshot> Snapshots { get; } = [];

    public Snapshot CreateSnapshot(string description)
    {
        Snapshot snap = new(
            ++_lastSnapshotId,
            [.. session.Match.Players.Select(p => p.GetRecord())],
            description,
            session.Match
        );
        Snapshots.AddLast(snap);

        while (Snapshots.Count > session.Config.SnapshotMemory)
            Snapshots.RemoveFirst();
            
        return snap;
    }

    public Snapshot GetAndClear(int id)
    {
        var result = Snapshots.Single(s => s.Id == id);
        Snapshots.Clear();
        return result;
        // while (true)
        // {
        //     var result = Snapshots.Last();
        //     Snapshots.RemoveLast();
        //     if (result.Id == id) return result;
        //     if (Snapshots.Count == 0)
        //         throw new CodeErrorException($"Failed to find snapshot with Id = {id}");
        // }
    }
}