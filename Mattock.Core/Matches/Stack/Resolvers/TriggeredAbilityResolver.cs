using System.Net.Mail;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Triggered;

namespace Mattock.Core.Matches.Stack.Resolvers;

// TODO a lot of shared code with ActivatedAbilityResolver
public class TriggeredAbilityResolver(
    TriggeredAbility ta
) : IStackEffectResolver
{
    public TriggeredAbility Ability { get; } = ta;

    public async Task<RollbackRequest?> Resolve(StackEffect effect)
    {
        foreach (var e in Ability.Effects)
        {
            var rollback = e.Do(effect.Ctx);
            if (rollback is not null)
                return rollback;
        }

        return null;
    }

    public bool IsCard(Card card) => false;
}