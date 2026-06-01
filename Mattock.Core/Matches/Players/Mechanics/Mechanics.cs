using Mattock.Core.Matches.Players.Mechanics.Mulligans;

namespace Mattock.Core.Matches;

public class Mechanics
{
    public IMulliganRule? Mulligan { get; set; } = new LondonMulliganRule();
}