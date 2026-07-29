using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Scripting.Context;

public class EffectContext(
    IEffectContextData data,
    TargetDeclarationCollection targets
)
{
    
    public IEffectContextData Data { get; } = data;

    public TargetDeclarationCollection Targets { get; } = targets;

    // TODO mem
}