using Mattock.Core.Matches.Turns.Steps;
using Mattock.Core.Matches.Turns.Steps.Combat;

namespace Mattock.Core.Matches.Turns.Phases;

public class CombatPhase : Phase
{
    public CombatPhase(Match match) : base(match, PhaseType.Combat, [])
    {
        Steps.Add(new BeginningOfCombatStep(this));
        Steps.Add(new DeclareAttackersStep(this));

        // TODO declare blockers step
        // TODO combat damage steps

        Steps.Add(new EndOfCombatStep(this));
    }
}