using Mattock.Core.Matches.Turns.Steps.Beginning;
using Mattock.Core.Matches.Turns.Steps.Ending;

namespace Mattock.Core.Matches.Turns.Phases;

public class EndingPhase
    : Phase
{
    public EndingPhase(Match match)
        : base(match, PhaseType.Ending, [])
    {
        Steps.Add(new EndStep(this));
        Steps.Add(new CleanupStep(this));
    }
}
