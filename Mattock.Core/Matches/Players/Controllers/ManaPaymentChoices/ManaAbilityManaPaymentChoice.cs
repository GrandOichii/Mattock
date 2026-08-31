using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

public class ManaAbilityManaPaymentChoice(
    Player player,
    ActivatedAbility aa
) : IManaPaymentChoice
{
    public ActivatedAbility Ability { get; } = aa;

    public async Task<RollbackRequest?> Process(ManaPayment payment)
    {
        return await player.Activate(Ability);
    }

    public string ToDisplayString()
        => $"Activate mana ability {Ability.ActivatedAbilityId}";
}