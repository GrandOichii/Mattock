using Mattock.Core.Matches.Turns.Steps.Beginning;

namespace Mattock.Core.Matches.Turns.Phases;

public class BeginningPhase
    : Phase
{
    public BeginningPhase(Match match)
        : base(match, PhaseType.Beginning, [])
    {
        Steps.Add(new UntapStep(this));
        Steps.Add(new UpkeepStep(this));
        Steps.Add(new DrawStep(this));
    }
}
