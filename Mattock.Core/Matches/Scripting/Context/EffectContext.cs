using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Scripting.Context;

public class EffectContext(
    Player controller,
    IEffectContextData data,
    TargetDeclarationCollection targets
)
{
    public Player Controller { get; } = controller;
    
    public IEffectContextData Data { get; } = data;

    public TargetDeclarationCollection Targets { get; } = targets;

    // TODO mem
}