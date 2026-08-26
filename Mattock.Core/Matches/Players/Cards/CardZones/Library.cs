namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Library(
    Player player
) : OwnedCardZone(player)
{
    public override string GetZoneName() => "Library";
}