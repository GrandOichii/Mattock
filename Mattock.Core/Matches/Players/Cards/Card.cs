using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Matches.Players.Cards;

public class Card
{
    public Match Match { get; }
    public int OwnedIdx { get; }
    public string Id { get; }
    public CardTemplate Template { get; }

    public Card(Player owner, CardTemplate template)
    {
        Match = owner.Match;
        OwnedIdx = owner.Idx;
        Template = template;

        Id = Match.GenerateCardId();
    }

    public string GetDisplayName() => $"{{{Id}}}"; // TODO
}