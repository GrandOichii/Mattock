namespace Mattock.Core.Matches;

public class Rng(
    int seed
)
{
    private Random _r = new(seed);
    private int _c = 0;

    public int Next()
    {
        ++_c;
        return _r.Next();
    }
}