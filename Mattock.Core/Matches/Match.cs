using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches;

public class Match
{
    // properties
    public Random Rng { get; }
    public MatchConfig Config { get; }
    public Player[] Players { get; }
    public int ActivePlayerIdx { get; private set; }
    public Battlefield Battlefield { get; }
    private int _lastCardId;
    public CardZoneChange? ZoneChange { get; private set; }


    // constructors

    public Match(
        MatchConfig config,
        PlayerSetup[] playerSetups
    )
    {
        Config = config;
        Players = [.. playerSetups.Select(
            (s, idx) => new Player(this, idx, s)
        )];
        Rng = config.RandomMatch
            ? new()
            : new(config.Seed);
        ActivePlayerIdx = config.RandomFirstPlayer
            ? Rng.Next() % Players.Length
            : config.FirstPlayerIdx;

        Battlefield = new(this);

        _lastCardId = 0;
        ZoneChange = null;
    }

    public Player GetActivePlayer() => Players[ActivePlayerIdx];

    // methods

    public async Task Run()
    {
        // Game start

        // Choose the first active player
        var active = GetActivePlayer();
        var chosenActivePlayer = await active.Setup.Controller.ChoosePlayer(
            active,
            [.. Players ],
            "Choose the active player"
        );
        ActivePlayerIdx = chosenActivePlayer.Idx;

        // Set life totals
        foreach (var player in Players)
        {
            player.Life.Set(Config.StartingLifeTotal);
        }

        // Form player decks

        foreach (var player in Players)
        {
            player.FormDeck();
        }

        // Draw initial hand

        foreach (var player in Players)
        {
            player.Draw(Config.InitialHandSize);
        }

        // TODO
    }

    public string GenerateCardId() => $"c{++_lastCardId}";

    public void MoveCard(
        Card card,
        ICardZone fromZone,
        ICardZone toZone,
        CardZoneChangeType type
    )
    {
        ZoneChange = new(card, fromZone, toZone, type);

        // TODO apply all zone change replacement effects

        ZoneChange.Process();
        ZoneChange = null;
    }
}