

using Mattock.Core.Matches.Players.Cards;

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

public class CastSpellCommand(Player player, Card card) : ICommand
{
    public async Task Do()
    {
        await player.Cast(card);
    }

    public string ToCommandString() => $"{CastSpellAction.ActionWord} {card.Id}";
}