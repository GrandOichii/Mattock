namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Deck : OwnedCardZone
{
    public Deck(Player player) : base(player)
    {
    }

    public override string GetZoneName() => "Deck";
}