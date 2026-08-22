using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Events;

public class ChooseTargetsForSpellEvent(
    Card card,
    EffectContext ctx
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        var targets = card.GetSpellTargets();
        TargetDeclaration[] declarations = [.. targets.Select(t => t.Get(ctx))];
        ctx.Targets.AddRange(declarations);

        return null;
    }
}