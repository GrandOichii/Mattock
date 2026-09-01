using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Players.Actions;

public class CastSpellAction : IAction
{
    public static readonly string ActionWord = "Cast";

    public List<ICommand> GetAvailable(Player player)
    {
        var castableCards = player.GetCastableCards();

        return [.. castableCards.Select(c => new CastSpellCommand(player, c)) ];
    }
}

public class CastSpellCommand(
    Player player,
    Card card
) : ICommand
{
    public async Task<RollbackRequest?> Do()
    {
        var rollback = await player.Cast(card);
        if (rollback is not null)
            return rollback;

        player.Match.ResetPriority(player.Idx);
        return null;
    }

    public string ToCommandString() => $"{CastSpellAction.ActionWord} {card.Id}";
}