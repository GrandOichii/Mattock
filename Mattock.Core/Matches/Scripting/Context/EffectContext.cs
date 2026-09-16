using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Context;

public class EffectContext(
    Player controller,
    IEffectContextData data,
    TargetDeclarationCollection targets,
    LuaTable? memory = null
)
{
    public Player Controller { get; } = controller;
    
    public IEffectContextData Data { get; } = data;

    public LuaTable Memory { get; } = memory ?? LuaCommon.CreateTable(controller.Match.LState);

    public TargetDeclarationCollection Targets { get; } = targets;

    // TODO mem
}