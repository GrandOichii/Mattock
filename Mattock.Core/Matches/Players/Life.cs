namespace Mattock.Core.Matches.Players;

public class Life(Player player)
{
    public int Current { get; private set; }

    public void SetRaw(int v)
    {
        Current = v;
    }

    public void Set(int v)
    {
        // TODO
        SetRaw(v);
    }
}