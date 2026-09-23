using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Scripting.Continuous;

public class ContinuousEffect(
    // TODO
    Card source,
    int timestamp
)
{
    // TODO
    public Card Source { get; } = source;
    public int Timestamp { get; } = timestamp;

    public object[] GetEffectedObjects()
    {
        throw new NotImplementedException();
    }
}

// Dependency examples:

// Blood Moon + Tomb of Yawgmoth
// Blood Moon takes away the ability of Tomb of Yawgmoth, which means that Tomb of Yawgmoth depends on Blood Moon
// Due to this, Tomb of Yawgmoth will apply only after Blood Moon takes effect, which leads to Urborg will be a Legendary Mountain

// Grizzly Bear + Control Magic + Steal Enchantment
// (Control Magic - Enchant creature You control enchanted creature)
// (Steal Enchantment - Enchant Enchantment You control enchanted Enchantment)
// Player A casts Grizzly Bear. Player B casts Control Magic, stealing the creature. Player A casts Steal Enchantment, targeting Control Magic. 
// Who controls Grizzly Bear? Steal Enchantment effects Control Magic, which means Control Magic depends on Steal Enchantment. 
// Thus the continuous effect of Steal Enchantment will apply first, changing the controller of Control Magic. Then effect of Control Magic will apply, changing the controller of Grizzly Bears to player A.
