using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class Permanent
{
    public Match Match { get; }
    public Card Card { get; }
    public Player? Controller { get; private set; }

    private bool _tapped;

    public Permanent(Card card)
    {
        Match = card.Match;
        Card = card;
        Controller = null;
        _tapped = false;
    }

    public Player GetOwner() => Match.Players[Card.OwnerIdx];

    public void SetController(Player controller)
    {
        Controller = controller;
    }

    public bool IsLand()
    {
        // TODO
        return Card.IsLand();
    }

    public bool IsControlledBy(int playerIdx)
    {
        // TODO?
        return Controller!.Idx == playerIdx;
    }

    public bool IsUntapped() => !_tapped;

    public bool IsTapped() => _tapped;

    public bool HasName(string name)
    {
        // TODO 
        return Card.HasName(name);
    }   
}