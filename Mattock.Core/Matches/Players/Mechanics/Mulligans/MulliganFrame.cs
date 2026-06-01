namespace Mattock.Core.Matches.Players.Mechanics.Mulligans;

public class MulliganFrame
{
    public Player Player { get; }
    public bool WillTakeMulligan { get; set; }
    public int MulligansTaken { get; private set; }

    public MulliganFrame(Player player)
    {
        Player = player;
        WillTakeMulligan = true;
        MulligansTaken = 0;
    }

    public async Task Do()
    {
        if (!WillTakeMulligan) return;

        await Player.Match.Mechanics.Mulligan!.Do(Player, this);

        ++MulligansTaken;
    }
}