using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Matches.Turns;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches;

public class Match
{
    // properties
    public Random Rng { get; }
    public MatchConfig Config { get; }
    public Mechanics Mechanics { get; }
    public Player[] Players { get; }
    public Battlefield Battlefield { get; }
    private int _lastCardId;
    public CardZoneChange? ZoneChange { get; private set; }
    public TheStack Stack { get; }
    public TurnOrderManager TurnOrderManager { get; }
    public Priority? Priority { get; private set; }
    public TurnManager TurnManager { get; }
    public IAction[] Actions { get; }
    public int TurnCounter { get; private set; }
    public List<Card> Cards { get; }

    // constructors

    public Match(
        MatchConfig config,
        PlayerSetup[] playerSetups,
        Mechanics mechanics
    )
    {
        Config = config;
        Mechanics = mechanics;

        TurnOrderManager = new(this);
        ZoneChange = null;
        Priority = null;
        Stack = new(this);
        Battlefield = new(this);
        TurnManager = new(this);
        _lastCardId = 0;
        TurnCounter = 0;
        Cards = [];

        Players = [.. playerSetups.Select(
            (s, idx) => new Player(this, idx, s)
        )];
        Rng = config.RandomMatch
            ? new()
            : new(config.Seed);
        TurnOrderManager.ActivePlayerIdx = config.RandomFirstPlayer
            ? Rng.Next() % Players.Length
            : config.FirstPlayerIdx;

        Actions = [
            new PassAction(),
            new PlayLandSpecialAction()
        ];
    }

    public Player GetActivePlayer() => Players[TurnOrderManager.ActivePlayerIdx];

    // methods

    public async Task Run()
    {
        // Game start

        // Choose the first active player
        var active = GetActivePlayer();
        var chosenActivePlayer = await active.ChoosePlayer(
            [.. Players ],
            "Choose the active player"
        );
        TurnOrderManager.ActivePlayerIdx = chosenActivePlayer.Idx;

        // Set life totals
        foreach (var player in Players)
        {
            player.Life.Set(Config.StartingLifeTotal);
        }

        // Form player libraries

        foreach (var player in Players)
        {
            player.FormLibrary();
        }

        // Draw initial hand

        foreach (var player in Players)
        {
            player.Draw(Config.InitialHandSize);
        }

        // Mulligans

        await TakeMulligans();

        // TODO
        await TakeTurns();
    }

    public async Task TakeTurns()
    {
        while (!GameEnded())
        {
            ++TurnCounter;
            
            for (
                TurnManager.Reset();
                !TurnManager.IsTurnEnded();
                TurnManager.Advance()
            )
            {
                var phase = TurnManager.GetCurrentPhase();

                await phase.DoPrePhases();
                await phase.DoPhases();
                await phase.DoPostPhases();
            }

            TurnManager.Reset();
            TurnOrderManager.Advance();

            foreach (var p in Players)
                p.ResetTrackers();
        }
    }

    public void CreatePriority()
    {
        Priority = new(this);
    }

    public async Task CreateAndResolvePriority()
    {
        CreatePriority();

        await Priority!.Resolve();

        Priority = null;
    }

    public void ResetPriority()
    {
        if (Priority is null)
            throw new Exception($"Called {nameof(ResetPriority)} while no priority is present!");
        Priority.Reset();
    }

    public bool GameEnded()
    {
        // TODO
        return false;
    }

    public async Task TakeMulligans()
    {
        if (Mechanics.Mulligan is null) return;

        MulliganFrame[] mulliganFrames = [.. Players.Select(p => new MulliganFrame(p))];
        while (mulliganFrames.Any(f => f.WillTakeMulligan))
        {
            foreach (var f in mulliganFrames)
            {
                if (!f.WillTakeMulligan) continue;
                var resp = await f.Player.ChooseString([ "Yes", "No" ], "Mulligan?");
                f.WillTakeMulligan = resp == "Yes";
            }

            foreach (var f in mulliganFrames)
            {
                await f.Do();
            }
        }
    }

    public string GenerateCardId(Card card) {
        Cards.Add(card);
        return $"c{++_lastCardId}";
    }

    public Card GetCardById(string id) => Cards.Single(c => c.Id == id);

    public void MoveCard(
        Card card,
        ICardZone toZone,
        CardZoneChangeType type
    )
    {
        ZoneChange = new(card, toZone, type);

        // TODO apply all zone change replacement effects

        ZoneChange.Process();
        ZoneChange = null;
    }

    public List<ICommand> GetAvailableCommands(Player player)
    {
        List<ICommand> result = [];

        foreach (var action in Actions)
        {
            result.AddRange(action.GetAvailable(player));
        }

        if (result.Count == 0)
        {
            throw new Exception($"Code error: no available commands for player {player.GetDisplayName()}");
        }

        return result;
    }

    public async Task UpdateExcept(Player player)
    {
        foreach (var p in Players)
        {
            if (p == player) continue;
            await p.Controller.Update(player, $"Waiting for {player.GetDisplayName()}");
        }
    }

    public async Task PutOntoTheBattlefield(Card card, Player controller)
    {
        await Battlefield.MoveCard(card, controller);
    }
}