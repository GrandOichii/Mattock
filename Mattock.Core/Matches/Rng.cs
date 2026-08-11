using Mattock.Core.Matches.Snapshots;

namespace Mattock.Core.Matches;

public class Rng(
    int seed
) : IHasSnapshot<Rng.Snapshot>
{
    private readonly Random _r = new(seed);
    private int _c = 0;

    public int Next()
    {
        ++_c;
        return _r.Next();
    }

    public Snapshot GetSnapshot()
    {
        return new()
        {
            Count = _c
        };
    }

    public void Restore(Snapshot s)
    {
        _c = s.Count;
    }

    public class Snapshot
    {
        public required int Count { get; init; }
    }
}