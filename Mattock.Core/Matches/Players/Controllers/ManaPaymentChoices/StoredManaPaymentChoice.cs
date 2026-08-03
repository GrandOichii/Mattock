using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

public class StoredManaPaymentChoice(
    StoredMana mana,
    ManaPool pool
) : IManaPaymentChoice
{
    public StoredMana Mana { get; } = mana;
    public ManaPool Pool { get; } = pool;

    public Task Process(ManaPayment payment)
    {
        Pool.Remove(Mana);
        payment.Pay(Mana);

        return Task.CompletedTask;
    }

    public string ToDisplayString()
        => $"Pay {Mana.Type}";
}