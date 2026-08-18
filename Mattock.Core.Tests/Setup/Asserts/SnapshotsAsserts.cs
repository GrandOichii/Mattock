using System.Diagnostics;
using Mattock.Core.Matches.Snapshots;

namespace Mattock.Core.Tests.Setup.Asserts;

public class SnapshotsAsserts(
    SnapshotsManager manager
)
{
    public SnapshotsAsserts HasCount(int v)
    {
        manager.Snapshots.Count.ShouldBe(v);
        return this;
    }

    public SnapshotsAsserts AssertSnapshot(int idx, Action<SnapshotAsserts> a)
    {
        a(new(manager.Snapshots[idx]));
        return this;
    }
}