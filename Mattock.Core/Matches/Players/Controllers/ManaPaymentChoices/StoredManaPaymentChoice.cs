using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

public class StoredManaPaymentChoice(
    StoredMana mana,
    ManaPool pool
) : IManaPaymentChoice
{
    public StoredMana Mana { get; } = mana;
    public ManaPool Pool { get; } = pool;

    public Task<RollbackRequest?> Process(ManaPayment payment)
    {
        Pool.Remove(Mana);
        payment.Pay(Mana);

        return Task.FromResult<RollbackRequest?>(null);
    }

    public string ToDisplayString()
        => $"Pay {Mana.Type}";
}