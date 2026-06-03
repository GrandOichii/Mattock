using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public class StoredMana
{
    public ManaType Type { get; }
    public int Amount { get; private set; }

    public StoredMana(ManaType type, int amount)
    {
        Type = type;
        Amount = amount;
    }

    public void Add(int amount)
    {
        Amount += amount;
    }
}