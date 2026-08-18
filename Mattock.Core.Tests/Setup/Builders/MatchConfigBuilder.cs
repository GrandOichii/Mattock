using Microsoft.VisualStudio.TestPlatform.ObjectModel;

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

    public MatchConfigBuilder MaxHandSize(int size)
    {
        _result.MaxHandSize = size;
        return this;
    }

    public MatchConfigBuilder NoMaxHandSize()
    {
        _result.MaxHandSize = null;
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

    public MatchConfigBuilder SnapshotMemory(int v)
    {
        _result.SnapshotMemory = v;
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
    
    public MatchConfigBuilder NoManaPoolEmptying()
    {
        _result.ManaPoolEmptiesAtEndOfEachPhase = false;
        _result.ManaPoolEmptiesAtEndOfEachStep = false;
        return this;
    }

    public MatchConfigBuilder NoSummoningSickness()
    {
        _result.SummoningSickness = false;
        return this;
    }

    public MatchConfigBuilder BaselineBlockCount(int v)
    {
        _result.BaselineBlockCount = v;
        return this;
    }

    public MatchConfigBuilder NoMaxLandsPerTurn()
    {
        _result.MaxLandsPerTurn = null;
        return this;
    }
}