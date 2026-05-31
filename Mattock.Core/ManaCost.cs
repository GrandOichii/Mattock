using Mattock.Core.Matches.Mana;

namespace Mattock.Core;

public class ManaCost
{
    public required ManaType Type { get; init; }
    public required int Amount { get; init; }
}