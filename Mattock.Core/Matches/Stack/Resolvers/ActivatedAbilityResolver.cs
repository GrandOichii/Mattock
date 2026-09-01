using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack.Resolvers;

public class ActivatedAbilityResolver(
    ActivatedAbility aa
) : IStackEffectResolver
{
    public async Task<RollbackRequest?> Resolve(StackEffect effect)
    {
        foreach (var e in aa.Effects)
        {
            var rollback = e.Do(effect.Ctx);
            if (rollback is not null)
                return rollback;
        }

        return null;
    }

    public bool IsCard(Card card) => false;
}