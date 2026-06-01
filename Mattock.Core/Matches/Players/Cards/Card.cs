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

    public bool IsLand()
    {
        // TODO
        return Template.Types.Contains("Land");
    }

    public bool IsSorcery()
    {
        // TODO
        return Template.Types.Contains("Sorcery");
    }

    public bool IsInstant()
    {
        // TODO
        return Template.Types.Contains("Instant");
    }
}