namespace Mattock.Core.Matches.Players.Cards.CardZones;

public class Graveyard : OwnedCardZone
{
    public Graveyard(Player player) : base(player)
    {
    }

    public override string GetZoneName() => "Graveyard";
}