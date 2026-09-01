using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class PutOntoTheBattlefieldEvent(
    (Card card, Player controller)[] pairs
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        foreach (var (card, controller) in pairs)
        {
            var (_, request) = await match.Battlefield.MoveCard(card, controller);
            if (request is not null)
                return request;
        }

        return null;
    }
}

