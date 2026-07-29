namespace Mattock.Core.Matches.Scripting.Targets;

public class TargetDeclarationCollection(
    TargetDeclaration[] declarations
)
{
    public TargetDeclaration Get(string tgtKey)
    {
        return declarations.Single(t => t.Key == tgtKey);
    }
}