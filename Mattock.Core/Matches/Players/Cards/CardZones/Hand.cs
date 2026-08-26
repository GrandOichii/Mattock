namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Hand(
    Player player
) : OwnedCardZone(player)
{
    public override string GetZoneName() => "Hand";
}