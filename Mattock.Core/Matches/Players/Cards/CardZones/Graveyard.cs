namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Graveyard(
    Player player
) : OwnedCardZone(player)
{
    public override string GetZoneName() => "Graveyard";
}