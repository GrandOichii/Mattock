using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

public class DestroyEvent(
    Permanent[] _permanents
): IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        // List<Permanent> destroyed = [];
        foreach (var permanent in _permanents)
        {
            var rollback = await Destroy(permanent);
            if (rollback is not null)
                return rollback;

        }

        // TODO trigger

        return null;
    }

    private async Task<RollbackRequest?> Destroy(Permanent permanent)
    {
        // TODO very basic
        var controller = permanent.GetController();

        var changer = controller.Graveyard.GetCardZoneChanger();

        // TODO check if can be destroyed
        if (permanent.HasType(CardTypes.Creature))
        {
            permanent.Match.Triggers.Process(new(
                TriggerType.SingleDeath,
                new SingleDeathTriggerContext(permanent)
            ));
        }

        var (_, request) = await controller.Match.MoveCard(
            permanent.Card,
            Zones.CardZoneChangeType.Top,
            changer
        );

        if (request is not null)
            return request;

        return null;
    }
}