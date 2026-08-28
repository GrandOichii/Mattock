using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class IdManager(
    Match match
)
{
    private int _lastCardId = 0;
    private int _lastActivatedAbilityId = 0;
    private int _lastPermanentId = 0;
    private int _lastStackEffectId = 0;

    public string GenerateStackEffectId()
        => $"StackEffect[{++_lastStackEffectId}]";
    
    public string GenerateActivatedAbilityId()
        => $"ActivatedAbility[{++_lastActivatedAbilityId}]";

    public string GenerateCardId(Card card) {
        match.Cards.Add(card);
        return $"Card[{++_lastCardId}]";
    }

    public string GetPlayerId(Player player)
        => $"Player[{player.Idx}]";

    public string GeneratePermanentId()
        => $"Permanent[{++_lastPermanentId}]";
}

