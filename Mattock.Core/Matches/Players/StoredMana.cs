using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players;

public class StoredMana
{
    public ManaType Type { get; }

    public StoredMana(ManaType type)
    {
        Type = type;
    }
}