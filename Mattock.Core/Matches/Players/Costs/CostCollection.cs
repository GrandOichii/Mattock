using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Costs;

public class CostCollection
{
    public required string Text { get; init; }
    public required ManaCost[] ManaCosts { get; init; }
    // TODO

    public bool CanBePayed(Player player)
    {
        // TODO some mana restricts what it can be used for
        List<ManaType?> manaTypes = [
            ManaType.White,
            ManaType.Blue,
            ManaType.Black,
            ManaType.Red,
            ManaType.Green,
            ManaType.Colorless,
            null,
        ];
        var store = player.ManaPool.CreateStore();

        // colored mana
        foreach (var manaType in manaTypes)
        {
            var costs = ManaCosts.Where(c => c.Type == manaType);
            if (!costs.All(store.CanPayFor))
                return false;
        }
        return true;
    }

    public Queue<ManaCost> GetManaCosts() => new(ManaCosts.Select(c => new ManaCost() {
        Amount = c.Amount,
        Type = c.Type
    }));
}