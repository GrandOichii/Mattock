using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Setup;

public class PlayerSetup
{
    public required string Name { get; init; }
    public required IPlayerController Controller { get; init; }
    public required DeckTemplate Deck { get; init; }
    public required int TeamIdx { get; init; }
}