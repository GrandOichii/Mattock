

using System.Reflection;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Players.Actions;

// 116.2a Playing a land is a special action. To play a land, a player puts that land onto the battlefield from the zone it was in (usually that player’s hand). By default, a player can take this action only once during each of their turns. A player can take this action any time they have priority and the stack is empty during a main phase of their turn. See rule 305, “Lands.”

// 305.1. A player who has priority may play a land card from their hand during a main phase of their turn when the stack is empty. Playing a land is a special action; it doesn’t use the stack (see rule 116). Rather, the player simply puts the land onto the battlefield. Since the land doesn’t go on the stack, it is never a spell, and players can’t respond to it with instants or activated abilities.

// 305.2. A player can normally play one land during their turn; however, continuous effects may increase this number.

// 305.3. A player can’t play a land, for any reason, if it isn’t their turn. Ignore any part of an effect that instructs a player to do so.

// 305.9. If an object is both a land and another card type, it can be played only as a land. It can’t be cast as a spell.

public class PlayLandSpecialAction : IAction
{
    public static readonly string ActionWord = "PlayLand";

    public List<ICommand> GetAvailable(Player player)
    {
        var match = player.Match;

        // Player has to be the active player
        if (match.TurnManager.ActivePlayerIdx != player.Idx) return [];

        // Phase has to be a main phase
        var phase = match.TurnManager.GetCurrentPhase();
        if (!phase.IsMainPhase()) return [];

        // Has to not have already played a land this turn (can be changed)
        var max =  player.GetMaxLandsPerTurn();
        if (max is not null && player.LandsPlayedThisTurn >= max) return [];

        // Has to have a land in their hand (not always)
        var lands = player.GetPlayableLands();
        return [ .. lands.Select(l => new PlayLandCommand(l)) ];
    }
}

public class PlayLandCommand(Card card) : ICommand
{
    public async Task Do()
    {
        var match = card.Match;
        var player = match.GetActivePlayer();
        ++player.LandsPlayedThisTurn;
        await match.PutOntoTheBattlefield(
            card,
            match.GetActivePlayer()
        );
    }

    public string ToCommandString() => $"{PlayLandSpecialAction.ActionWord} {card.Id}";
}