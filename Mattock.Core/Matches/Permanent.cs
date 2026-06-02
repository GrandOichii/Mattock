using System.Reflection.Metadata.Ecma335;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class Permanent
{
    public string Pid { get; }
    public Match Match { get; }
    public Card Card { get; }
    public Player? Controller { get; private set; }

    private bool _tapped;

    public Permanent(Card card)
    {
        Pid = card.Match.Battlefield.GeneratePid();
        Match = card.Match;
        Card = card;
        Controller = null;
        _tapped = false;
    }

    public string GetDisplayName() => $"[{Pid}]";

    public Player GetOwner() => Match.Players[Card.OwnerIdx];

    public void SetController(Player controller)
    {
        Controller = controller;
    }

    public bool HasType(string type)
    {
        // TODO
        return Card.HasType(type);
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