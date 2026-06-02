using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Matches.Players.Cards;

public class Card
{
    public Match Match { get; }
    public int OwnerIdx { get; }
    public string Id { get; }
    public CardTemplate Template { get; }
    public ICardZone? Zone { get; private set; }

    public Card(Player owner, CardTemplate template)
    {
        Match = owner.Match;
        OwnerIdx = owner.Idx;
        Template = template;
        Zone = null;

        Id = Match.GenerateCardId(this);
    }

    public string GetDisplayName() => $"{{{Id}}}"; // TODO

    public void SetZone(ICardZone zone)
    {
        Zone = zone;
    }

    public bool HasName(string name)
    {
        // TODO
        return Template.Name == name;
    }

    public bool HasType(string type)
    {
        // TODO
        return Template.Types.Contains(type);
    }

    public bool IsLand() => HasType("Land");

    public bool IsSorcery() => HasType("Sorcery");

    public bool IsInstant() => HasType("Instant");

    public List<ManaCost> GetManaCosts()
    {
        // TODO
        return [ .. Template.ManaCosts ];
    }

    public bool CanBePlayedAsLand(Player player)
    {
        if (!IsLand()) return false;

        // TODO
        return Zone == player.Hand;
    }

    public bool CanBeCast(Player player)
    {
        if (IsLand())
            return false;

        if (!CardTypes.Castable.Any(HasType))
            return false;

        // TODO alternative costs
        var costs = GetManaCosts();
        if (costs.Count == 0)
            return false;

        // TODO this is very basic, change later
        if (!IsInstant() && !(Match.TurnManager.GetCurrentPhase().IsMainPhase() && player.IsActive()))
            return false; 

        if (Zone != player.Hand)
            return false;

        return true;
    }
}