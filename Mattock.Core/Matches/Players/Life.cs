namespace Mattock.Core.Matches.Players;

public class Life(Player player)
{
    public int Current { get; private set; }

    public void Set(int v)
    {
        Current = v;
    }
}