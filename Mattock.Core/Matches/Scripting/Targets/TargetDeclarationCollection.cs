namespace Mattock.Core.Matches.Scripting.Targets;

public class TargetDeclarationCollection(
    TargetDeclaration[] declarations
)
{
    public List<TargetDeclaration> Declarations { get; } = [.. declarations];

    public TargetDeclaration Get(string tgtKey)
    {
        return Declarations.Single(t => t.Key == tgtKey);
    }

    public void AddRange(TargetDeclaration[] declarations)
    {
        Declarations.AddRange(declarations);
    }
}