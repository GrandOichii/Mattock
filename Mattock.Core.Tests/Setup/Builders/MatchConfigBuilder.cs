namespace Mattock.Core.Tests.Setup.Builders;

public class MatchConfigBuilder
{
    private readonly MatchConfig _result = MatchConfig.Copy(MatchConfig.Default);

    public MatchConfig Build() => _result;

    public MatchConfigBuilder FirstPlayerIdx(int idx)
    {
        _result.FirstPlayerIdx = idx;
        _result.RandomFirstPlayer = false;
        return this;
    }

    public MatchConfigBuilder InitialHandSize(int size)
    {
        _result.InitialHandSize = size;
        return this;
    }

    public MatchConfigBuilder GameLossIfRequiredToDrawFromEmptyDeck(bool v)
    {
        _result.GameLossIfRequiredToDrawFromEmptyDeck = v;
        return this;
    }
}