using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;

namespace Mattock.Core;

// TODO docs
public interface ICost
{
    // TODO docs
    bool CanPay(EffectContext ctx);

    /// <summary>
    /// Pay for the cost
    /// </summary>
    /// <param name="ctx">Effect context</param>
    /// <returns>true if a rollback was requested, otherwise false</returns>
    Task<RollbackRequest?> Pay(EffectContext ctx);
}