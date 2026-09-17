using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Triggers.Context;

public class SingleDrawTriggerContext(
    Player player,
    Card card
) : ITriggerContext
{
    public Player Player { get; } = player;
    public Card Card { get; } = card;
}