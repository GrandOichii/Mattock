namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Library : OwnedCardZone
{
    public Library(Player player) : base(player)
    {
    }

    public override string GetZoneName() => "Library";
}