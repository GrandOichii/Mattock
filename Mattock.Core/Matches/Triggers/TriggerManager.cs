namespace Mattock.Core.Matches.Triggers;

public class TriggerManager(
    Match match
)
{
    public List<QueuedTriggeredAbility> TriggerQueue { get; } = [];

    public void Process(Trigger trigger)
    {
        var cards = match.GetCardControllerPairs();
        foreach (var (card, controller) in cards)
        {
            var abilities = card.GetTriggeredAbilities();
            foreach (var ability in abilities)
            {
                var ctx = ability.CanTrigger(controller, card, trigger);
                if (ctx is null) continue;

                TriggerQueue.Add(new(
                    ability,
                    ctx
                ));
                // if (!ability.CanTrigger()) continue;

                // TODO
            }
        }
    }

    public QueuedTriggeredAbility[] PopTriggeredAbilityQueue()
    {
        QueuedTriggeredAbility[] result = [.. TriggerQueue];
        TriggerQueue.Clear();
        return result;
    }
}