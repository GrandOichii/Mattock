using Mattock.Core.Matches.Scripting.Context.Data;

namespace Mattock.Core.Matches.Scripting.Context;

public class EffectContext(
    IEffectContextData data
)
{
    public IEffectContextData Data { get; } = data;
    // TODO mem
}