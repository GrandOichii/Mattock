using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Events;

public class ChooseTargetsForSpellEvent(
    Player player,
    Card card,
    EffectContext ctx
) : IEvent
{
    public async Task Do(Match match)
    {
        var effects = card.GetSpellTargets();

    }
}