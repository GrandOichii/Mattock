using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Scripting.Context;

namespace Mattock.Core;

public interface ICost
{
    bool CanPay(EffectContext ctx);
    Task Pay(EffectContext ctx);
}