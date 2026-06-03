
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Stack.Resolvers;

public class SpellResolver : IStackEffectResolver
{
    public Match Match { get; }
    public Card Card { get; }

    public SpellResolver(Card card)
    {
        Match = card.Match;
        Card = card;
    }

    public async Task Resolve(StackEffect effect)
    {
        if (Card.IsPermanentType())
        {
            // 608.3a if no targets, Move from stack onto the battlefield
            var pid = Match.MoveCard(
                Card,
                Match.Battlefield,
                CardZoneChangeType.Bottom
            );
            if (pid is null) 
                return;
            var permanent = Match.Battlefield.GetPermanentByPid(pid)
                ?? throw new Exception($"Failed to fetch newly created permanent with PID = {pid} (card: {Card.GetDisplayName()})");
            permanent.SetController(effect.Controller!);

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
            Match.Players[Card.OwnerIdx].Graveyard,
            CardZoneChangeType.Top
        );

        // 608.2p Triggers
        // TODO
    }

    public bool IsCard(Card card) => Card == card;
}