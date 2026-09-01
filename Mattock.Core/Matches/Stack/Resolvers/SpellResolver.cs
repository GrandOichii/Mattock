using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack.Resolvers;

public class SpellResolver(
    Card card
) : IStackEffectResolver
{
    public Card Card { get; } = card;

    public async Task<RollbackRequest?> Resolve(StackEffect effect)
    {
        var match = Card.Match;
        RollbackRequest? rollback;
        if (Card.IsPermanentType())
        {
            // 608.3a if no targets, Move from stack onto the battlefield
            rollback = await match.Events.PutOntoTheBattlefield(
                [ (Card, effect.Ctx.Controller) ]
            );

            if (rollback is not null)
                return rollback;
            // var permanentId = await match.PutOntoTheBattlefield(Card, effect.Ctx.Controller);
            // if (permanentId is null) 
            //     return;

            // var permanent = match.Battlefield.GetPermanentByPermanentid(permanentId)
            //     ?? throw new CodeErrorException($"Failed to fetch newly created permanent with PermanentId = {permanentId} (card: {Card.GetDisplayName()})");

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

            return null;
        }

        // 608.2a Intervening "if" clause (603.4)
        // TODO

        // 608.2b Targets
        // TODO

        // 608.2c Execute effects
        rollback = await Card.ResolveSpellEffects(effect.Ctx);
        if (rollback is not null)
            return rollback;

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
        (_, rollback) = await match.MoveCard(
            Card,
            CardZoneChangeType.Top,
            match.Players[Card.OwnerIdx].Graveyard.GetCardZoneChanger()
        );

        if (rollback is not null)
            return rollback;

        // 608.2p Triggers
        // TODO

        return null;
    }

    public bool IsCard(Card card) => Card == card;
}