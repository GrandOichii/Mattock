using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Combat;

public class BlockDeclaration(
    Permanent blocker,
    Permanent[] attackers
)
{
    public Permanent Blocker { get; } = blocker;

    public Permanent[] Attackers { get; } = attackers;

    public void Apply()
    {
        foreach (var attacker in Attackers)
        {
            attacker.AddBlocker(Blocker);
        }
    }

    public string GetDisplayName()
        => $"{Blocker.GetDisplayName()} <- {string.Join(", ", Attackers.Select(a => a.GetDisplayName()))}";

    // TODO naming
    public bool MatchesShort(Short shortAD)
    {
        if (Blocker.PermanentId != shortAD.BlockerPermanentId) return false;
        
        for (int i = 0; i < Attackers.Length; ++i)
            if (Attackers[i].PermanentId != shortAD.AttackerPermanentIds[i])
                return false;

        return true;
    }

    // TODO naming
    public Short GetShort()
        => new()
        {
            BlockerPermanentId = Blocker.PermanentId,
            AttackerPermanentIds = [.. Attackers.Select(a => a.PermanentId)]
        };

    // TODO naming
    public class Short
    {
        public required string BlockerPermanentId { get; init; }
        public required string[] AttackerPermanentIds { get; init; }
    }

}