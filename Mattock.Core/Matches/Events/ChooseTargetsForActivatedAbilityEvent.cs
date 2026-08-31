using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Events;

public class ChooseTargetsForActivatedAbilityEvent(
    ActivatedAbility aa,
    EffectContext ctx
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        var targets = aa.GetTargets();
        TargetDeclaration[] declarations = new TargetDeclaration[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
        {
            var (dec, request) = targets[i].Get(ctx);
            if (request is not null)
                return request;
            declarations[i] = dec;
        }
        ctx.Targets.AddRange(declarations);

        return null;
    }
}