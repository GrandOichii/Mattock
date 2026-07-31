using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack.Resolvers;

public class SpellResolver(
    Card card
) : IStackEffectResolver
{
    public Match Match { get; } = card.Match;
    public Card Card { get; } = card;

    public async Task Resolve(StackEffect effect)
    {
        if (Card.IsPermanentType())
        {
            // 608.3a if no targets, Move from stack onto the battlefield
            var pid = await Match.PutOntoTheBattlefield(card, effect.Ctx.Controller);
            if (pid is null) 
                return;
            var permanent = Match.Battlefield.GetPermanentByPid(pid)
                ?? throw new Exception($"Failed to fetch newly created permanent with PID = {pid} (card: {Card.GetDisplayName()})");
            permanent.SetController(effect.Ctx.Controller!);

            // 608.3b Targets
            // TODO

            // 608.3c Auras
            // TODO

            // 608.3d Mutate
            // TODO

            // 608.3e If can't put onto the battlefield, put into the owner's graveyard
            // TODO

            // 608.3f copy of permanent spell
            // TODO

            // 608.3g 

            return;
        }

        // 608.2a Intervening "if" clause (603.4)
        // TODO

        // 608.2b Targets
        // TODO

        // 608.2c Execute effects
        await Card.ResolveSpellEffects(effect.Ctx);

        // TODO

        // 607.2d ???
        // TODO

        // 608.2e ???
        // TODO

        // 608.2f ???
        // TODO

        // 608.2g ???
        // TODO

        // 608.2h ???
        // TODO

        // 608.2i ???
        // TODO

        // 608.2j ???
        // TODO

        // 608.2k ???
        // TODO

        // 608.2m ???
        // TODO

        // 608.2n Move from stack to owner's graveyard
        Match.MoveCard(
            Card,
            CardZoneChangeType.Top,
            Match.Players[Card.OwnerIdx].Graveyard.GetCardZoneChanger()
        );

        // 608.2p Triggers
        // TODO
    }

    public bool IsCard(Card card) => Card == card;
}