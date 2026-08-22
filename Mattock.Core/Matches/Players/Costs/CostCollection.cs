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
            var request = await cost.Pay(ctx);
            if (request is not null)
                return request;
            if (ctx.Controller.Match.ShouldHalt())
                return null;
        }

        return null;
    }
}