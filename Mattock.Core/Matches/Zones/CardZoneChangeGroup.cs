using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Zones;

public class CardZoneChangeGroup(
    Match match,
    CardZoneChange[] zoneChanges
)
{
    public CardZoneChange[] ZoneChanges { get; } = zoneChanges;

    public async Task<(CardZoneChangeResult[], RollbackRequest?)> Process()
    {
        List<CardZoneChangeResult> result = [];
        foreach (var zoneChange in ZoneChanges)
        {
            var (changeResult, rollback) = await zoneChange.Process();
            if (rollback is not null)
                return ([], rollback);
            result.Add(changeResult);
        }

        // FIXME this doesn't follow rule 613.7m
        foreach (var zoneChange in ZoneChanges)
        {
            zoneChange.Card.UpdateTimestamp();
        }

        // static abilities
        match.ContinuousEffects.UpdateEffectsFor([.. ZoneChanges.Select(c => c.Card)]);

        return ([.. result], null);
    }
}