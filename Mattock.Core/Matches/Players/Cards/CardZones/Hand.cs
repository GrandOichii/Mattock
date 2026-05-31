namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Hand : OwnedCardZone
{
    public Hand(Player player) : base(player)
    {
    }

    public override string GetZoneName() => "Hand";
}