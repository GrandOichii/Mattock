using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches.Players;

public class Player
{
    // properties

    public Match Match { get; }
    public int Idx { get; }
    public PlayerSetup Setup { get; }
    public IPlayerController Controller { get; }
    public Life Life { get; }

    // constructors

    public Player(
        Match match,
        int idx,
        PlayerSetup setup
    )
    {
        Match = match;
        Idx = idx;
        Setup = setup;
        Controller = setup.Controller;

        Life = new(this);
    }

    // methods

    public bool IsActive() => Idx == Match.ActivePlayerIdx;


    public bool IsNonActive() => Idx != Match.ActivePlayerIdx;


    public string GetDisplayName() => $"{Setup.Name} [{Idx}]";


    
}