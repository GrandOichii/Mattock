using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Events;

public class AddGenericManaEvent(
    Player[] players,
    ManaAmount[] mana
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var p in players)
        {
            foreach (var m in mana)
            {
                p.ManaPool.AddGenericMana((Mana.ManaType)m.Type!, m.Amount);
            }
        }

        return Task.CompletedTask;
    }
}