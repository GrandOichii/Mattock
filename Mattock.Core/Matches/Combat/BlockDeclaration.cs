using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Combat;

public class BlockDeclaration(
    Permanent blocker,
    Permanent[] attackers
)
{
    public Permanent Blocker { get; } = blocker;

    public Permanent[] Attackers { get; } = attackers;
}