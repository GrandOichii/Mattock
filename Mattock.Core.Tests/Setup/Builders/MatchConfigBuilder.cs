namespace Mattock.Core.Tests.Setup.Builders;

public class MatchConfigBuilder
{
    private MatchConfig _result = MatchConfig.Copy(MatchConfig.Default);

    public MatchConfig Build() => _result;

    public MatchConfigBuilder FirstPlayerIdx(int idx)
    {
        _result.FirstPlayerIdx = idx;
        _result.RandomFirstPlayer = false;
        return this;
    }
}