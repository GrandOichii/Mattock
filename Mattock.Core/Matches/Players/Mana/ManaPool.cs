using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public class ManaPool(Player player)
{
    public List<StoredMana> Mana { get; } = [];

    public bool IsEmpty() => Mana.Count == 0;

    public void AddGenericMana(ManaType type, int amount)
    {
        var mana = Mana.SingleOrDefault(m => m.Type == type);
        if (mana is null)
        {
            mana = new(type, 0);
            Mana.Add(mana);
        }

        mana.Add(amount);
    }

    public int GetTotal() => Mana.Sum(m => m.Amount);

    public void Clear()
    {
        Mana.Clear();
    }
}