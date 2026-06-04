using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public class ManaPool(Player player)
{
    public List<StoredMana> Mana { get; } = [];

    public bool IsEmpty() => Mana.Count == 0;

    public void AddGenericMana(ManaType type, int amount)
    {
        for (int i = 0; i < amount; ++i)
            Mana.Add(new(type));
        // var mana = Mana.SingleOrDefault(m => m.Type == type);
        // if (mana is null)
        // {
        //     mana = new(type, 0);
        //     Mana.Add(mana);
        // }

        // mana.Add(amount);
    }

    public int GetTotal() => Mana.Count;

    public void Clear()
    {
        Mana.Clear();
    }

    public ManaStore CreateStore() => new(this);

    public List<StoredMana> GetCandidates(ManaType? type)
    {
        return type is null
            ? [ .. Mana ]
            : [ .. Mana.Where(m => m.Type == type) ];
    }

    public void Remove(StoredMana choice)
    {
        if (Mana.Remove(choice)) return;

        throw new Exception($"Tried to remove non-existant stored mana of type {choice.Type} (\"{choice.Text}\") from mana pool of player {player.GetDisplayName()}");
    }
}

public class ManaStore
{
    public List<StoredMana> Mana { get; }

    public ManaStore(ManaPool manaPool)
    {
        Mana = [ .. manaPool.Mana ];
    }

    public bool CanPayFor(ManaCost cost)
    {
        for (int i = 0; i < cost.Amount; ++i)
        {
            var item = cost.Type is null
                ? Mana.FirstOrDefault()
                : Mana.FirstOrDefault(m => m.Type == cost.Type);

            if (item is null) return false;
            Mana.Remove(item);
        }

        return true;
    }
}