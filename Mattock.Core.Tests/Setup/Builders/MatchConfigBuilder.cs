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

    public MatchConfigBuilder TeamCount(int v)
    {
        _result.TeamCount = v;
        return this;
    }

    public MatchConfigBuilder MaxTeamSize(int v)
    {
        _result.MaxTeamSize = v;
        return this;
    }

    public MatchConfigBuilder GameLossIfRequiredToDrawFromEmptyLibrary(bool v)
    {
        _result.GameLossIfRequiredToDrawFromEmptyLibrary = v;
        return this;
    }

    public MatchConfigBuilder GameLossIfZeroOrLessLife(bool v)
    {
        _result.GameLossIfZeroOrLessLife = v;
        return this;
    }

    public MatchConfigBuilder FirstPlayerNoDrawIfSingleOpponent(bool v)
    {
        _result.FirstPlayerNoDrawIfSingleOpponent = v;
        return this;
    }
    
    public MatchConfigBuilder DisableManaPoolEmptying()
    {
        _result.ManaPoolEmptiesAtEndOfEachPhase = false;
        _result.ManaPoolEmptiesAtEndOfEachStep = false;
        return this;
    }
}