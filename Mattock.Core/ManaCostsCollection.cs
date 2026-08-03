using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Scripting.Context;

namespace Mattock.Core;

public class ManaCostsCollection(
    ManaCost[] manaCosts
) : ICost
{
    public bool CanPay(EffectContext ctx)
    {
        return true;
        // // TODO some mana restricts what it can be used for
        // List<ManaType?> manaTypes = [
        //     ManaType.White,
        //     ManaType.Blue,
        //     ManaType.Black,
        //     ManaType.Red,
        //     ManaType.Green,
        //     ManaType.Colorless,
        //     null,
        // ];
        // var store = ctx.Controller.ManaPool.CreateStore();

        // // colored mana
        // foreach (var manaType in manaTypes)
        // {
        //     var costs = manaCosts.Where(c => c.Type == manaType);
        //     if (!costs.All(store.CanPayFor))
        //         return false;
        // }

        // return true;
    }

    public async Task Pay(EffectContext ctx)
    {
        var player = ctx.Controller;

        var costs = GetManaPayment();
        while (!costs.PayedFor())
        {
            var types = costs.GetUnpayedTypes();
            StoredMana[] candidates = [.. types.SelectMany(t => player.ManaPool.GetCandidates(t)).Distinct()];

            var abilities = player.GetActivatableManaAbilities();

            IManaPaymentChoice[] options = [
                .. candidates.Select(c => new StoredManaPaymentChoice(c, player.ManaPool)),
                .. abilities.Select(a => new ManaAbilityManaPaymentChoice(player, a))
                // TODO add option for rollback
            ];

            // if (options.Length == 0)
            // {
            //     throw new Exception($"Code error: failed to find stored mana candidates to pay for mana cost"); // TODO type + better msg
            // }
            
            var choice = await player.ChooseManaPayment(options, $"Pay cost"); // TODO better hint
            await choice.Process(costs);
        }
    }

    private ManaPayment GetManaPayment() => new([.. manaCosts.Select(c => new ManaCost() {
        Amount = c.Amount,
        Type = c.Type
    })]);
}

public class ManaPayment(
    List<ManaCost> costs
)
{
    public List<PaymentItem> Costs { get; } = [.. costs.Where(c => c.Amount > 0).Select(c => new PaymentItem(c))];

    public bool PayedFor() => Costs.Count == 0;

    public ManaType?[] GetUnpayedTypes() => [.. Costs.Select(c => c.Type).Distinct()];

    public void Pay(StoredMana mana)
    {
        var best = Costs.FirstOrDefault(c => c.Type == mana.Type);
        best ??= Costs.FirstOrDefault(c => c.Type is null);
        if (best is null)
        {
            throw new Exception($"Failed to find best payment for provided stored mana: {mana.GetType()}"); // TODO type
        }
        --best.Amount;
        if (best.Amount == 0)
            Costs.Remove(best);
    }

    public class PaymentItem(ManaCost c)
    {
        public ManaType? Type { get; } = c.Type;
        public int Amount { get; set; } = c.Amount;
    }
}