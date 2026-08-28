namespace Mattock.Core.Matches.Snapshots;

public class SnapshotsManager(
    Session session
)
{
    private int _lastSnapshotId = 0;
    public LinkedList<Snapshot> Snapshots { get; } = [];

    public Snapshot CreateSnapshot()
    {
        // if (Snapshots.Any(s => s.Id == id))
        //     throw new CodeErrorException($"Snapshot with Id = {id} already exists");

        Snapshot snap = new(
            ++_lastSnapshotId,
            // session.GetMatchSnapshot(),
            [.. session.Match.Players.Select(p => p.GetRecord())],
            session.Match
        );
        Snapshots.AddLast(snap);

        while (Snapshots.Count > session.Config.SnapshotMemory)
            Snapshots.RemoveFirst();
            
        return snap;
    }

    public Snapshot DestructiveDequeue(int id)
    {
        while (true)
        {
            var result = Snapshots.Last();
            Snapshots.RemoveLast();
            if (result.Id == id) return result;
            if (Snapshots.Count == 0)
                throw new CodeErrorException($"Failed to find snapshot with Id = {id}");
        }
    }
}