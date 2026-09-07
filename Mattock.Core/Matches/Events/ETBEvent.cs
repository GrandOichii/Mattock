using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

public class ETBEvent(
    (Card card, Player controller)[] pairs
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        List<string> newPermanentIds = [];
        foreach (var (card, controller) in pairs)
        {
            var (permanentId, request) = await match.Battlefield.MoveCard(card, controller);
            if (request is not null)
                return request;
            if (permanentId is null) continue;
            newPermanentIds.Add(permanentId);
        }

        foreach (var permanentId in newPermanentIds)
        {
            var permanent = match.Battlefield.GetPermanentByPermanentid(permanentId);
            if (permanent is null)
                throw new CodeErrorException($"Failed to find permanent by it's permanent id after successfull move to the battlefield (permanent id: {permanentId})");
                
            match.Triggers.Process(new(
                TriggerType.ETB,
                new ETBTriggerContext(
                    permanent,
                    permanent.GetController()
                )
            ));
        }

        return null;
    }
}

