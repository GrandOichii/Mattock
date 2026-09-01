using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;

namespace Mattock.Core.Matches.Players.Costs;

public class CostCollection(
    string text,
    ICost[] costs
)
{
    public string Text { get; } = text;
    public ICost[] Costs { get; } = [.. costs];

    public bool CanBePayed(EffectContext ctx)
    {
        return costs.All(c => c.CanPay(ctx));
    }

    public async Task<RollbackRequest?> Pay(EffectContext ctx)
    {
        foreach (var cost in costs)
        {
            var rollback = await cost.Pay(ctx);
            if (rollback is not null)
                return rollback;
            if (ctx.Controller.Match.ShouldHalt())
                return null;
        }

        return null;
    }
}