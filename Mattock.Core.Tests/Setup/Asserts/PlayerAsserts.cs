namespace Mattock.Core.Tests.Setup.Asserts;

public class PlayerAsserts(Player player)
{
    public PlayerAsserts HasLife(int v)
    {
        player.Life.Current.ShouldBe(v);
        return this;
    }
}