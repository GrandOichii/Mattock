using Mattock.Core.Matches.Permanents.Statuses;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Permanents;

public class Permanent
{
    public string Pid { get; }
    public Match Match { get; }
    public Card Card { get; }
    public Player? Controller { get; private set; }

    public PermanentStatus Tapped { get; }
    public PermanentStatus Flipped { get; }
    public PermanentStatus FaceUp { get; }
    public PermanentStatus PhasedIn { get; }

    public Permanent(Card card)
    {
        Pid = card.Match.Battlefield.GeneratePid();
        Match = card.Match;
        Card = card;
        Controller = null;

        Tapped = new(PermanentStatusType.Tapped, false);
        Flipped = new(PermanentStatusType.Flipped, false);
        FaceUp = new(PermanentStatusType.FaceUp, true);
        PhasedIn = new(PermanentStatusType.PhasedIn, true);
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

    public bool IsUntapped() => !Tapped.Value;

    public bool IsTapped() => Tapped.Value;

    public bool HasName(string name)
    {
        // TODO 
        return Card.HasName(name);
    }
}