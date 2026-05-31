using Mattock.Core.Matches.Players.Controllers;

namespace Mattock.Core.Setup;

public class PlayerSetup
{
    public required string Name { get; init; }
    public required IPlayerController Controller { get; init; }
}