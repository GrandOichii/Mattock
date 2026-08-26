namespace Mattock.Core.Matches.Snapshots;

public interface IHasSnapshot<T>
{
    T GetSnapshot();
}