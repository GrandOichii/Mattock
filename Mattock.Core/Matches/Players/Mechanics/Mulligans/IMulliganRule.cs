namespace Mattock.Core.Matches.Players.Mechanics.Mulligans;

public interface IMulliganRule
{
    Task Do(Player player, MulliganFrame frame);
}