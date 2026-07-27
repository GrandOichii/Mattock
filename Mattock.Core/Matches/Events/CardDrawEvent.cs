using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Events;

public class CardDraw(
    Player player,
    int amount
)
{
    public void Do()
    {
        player.Draw(amount);
    }
}

public class CardDrawEvent(
    CardDraw[] draws
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var draw in draws)
        {
            draw.Do();
        }

        // TODO trigger
        
        return Task.CompletedTask;
    }
}