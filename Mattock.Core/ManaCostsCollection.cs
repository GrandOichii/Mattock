using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Scripting.Context;

namespace Mattock.Core;

public class ManaCostsCollection(
    ManaCost[] manaCosts
) : ICost
{
    public bool CanPay(EffectContext ctx)
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
        var store = ctx.Controller.ManaPool.CreateStore();

        // colored mana
        foreach (var manaType in manaTypes)
        {
            var costs = manaCosts.Where(c => c.Type == manaType);
            if (!costs.All(store.CanPayFor))
                return false;
        }

        return true;
    }

    public async Task Pay(EffectContext ctx)
    {
        var player = ctx.Controller;

        var manaCosts = GetManaCosts();
        while (manaCosts.Count > 0)
        {
            var manaCost = manaCosts.Dequeue();
            for (int i = 0; i < manaCost.Amount; ++i)
            {
                var candidates = player.ManaPool.GetCandidates(manaCost.Type);
                if (candidates.Count == 0)
                {
                    var postFix = manaCost.Type is null
                        ? "generic type"
                        : $"type {manaCost.Type}";
                    throw new Exception($"Code error: failed to find stored mana candidates to pay for mana cost of {postFix}");
                }

                
                var choice = await player.ChooseStoredMana([.. candidates], $"Pay cost"); // TODO better hint
                player.ManaPool.Remove(choice);
            }
        }
    }

    private Queue<ManaCost> GetManaCosts() => new(manaCosts.Select(c => new ManaCost() {
        Amount = c.Amount,
        Type = c.Type
    }));
}