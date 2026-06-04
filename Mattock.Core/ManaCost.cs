using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core;

public class ManaCost
{
    public required ManaType? Type { get; init; }
    public required int Amount { get; init; }

    public static ManaCost[] FromFormattedCost(string cost)
    {
        var mana = Mana.FromFormatted(cost);
        return [
            .. mana.Select(m => new ManaCost() {
                Amount = m.Amount,
                Type = m.Type
            })
        ];
    }
}