using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Matches.Stack;
using Mattock.Core.Matches.StateBasedActions;
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
    private readonly Dictionary<int, Player[]> _teams;
    public Battlefield Battlefield { get; }
    private int _lastCardId;
    public CardZoneChange? ZoneChange { get; private set; }
    public TheStack Stack { get; }
    public Priority? Priority { get; private set; }
    public TurnManager TurnManager { get; }
    public StateBasedActionsManager StateBasedActions { get; }
    public IAction[] Actions { get; }
    public int TurnCounter { get; private set; }
    public List<Card> Cards { get; }
    private int[]? _winningTeams;

    // constructors

    public Match(
        MatchConfig config,
        PlayerSetup[] playerSetups,
        Mechanics mechanics
    )
    {
        Config = config;
        Mechanics = mechanics;

        ZoneChange = null;
        Priority = null;
        Stack = new(this);
        Battlefield = new(this);
        TurnManager = new(this);
        StateBasedActions = new(this);
        _lastCardId = 0;
        TurnCounter = 0;
        Cards = [];
        _winningTeams = null;

        var nameGroupings = playerSetups.GroupBy(p => p.Name);
        foreach (var g in nameGroupings)
        {
            var c = g.Count();
            if (c == 1) continue;
            throw new DuplicatePlayerNameException($"Player name \"{g.Key}\" is repeated {c} times");
        }
        Players = [.. playerSetups.Select(
            (s, idx) => new Player(this, idx, s)
        )];
        _teams = playerSetups.Select(s => s.TeamIdx).Distinct().ToDictionary(
            tIdx => tIdx,
            tIdx => Players.Where(p => p.GetTeamIdx() == tIdx).ToArray()
        );
        if (_teams.Count > config.TeamCount)
            throw new TooManyTeamsException($"Too many teams were created (actual: {_teams.Count}, max: {config.TeamCount})");
        foreach (var (tIdx, players) in _teams)
            if (players.Length > config.MaxTeamSize)
                throw new TeamTooBigException($"Team with Idx = {tIdx} has too many players (actual: {players.Length}, max: {config.MaxTeamSize})");

        Rng = config.RandomMatch
            ? new()
            : new(config.Seed);
        TurnManager.ActivePlayerIdx = config.RandomFirstPlayer
            ? Rng.Next() % Players.Length
            : config.FirstPlayerIdx;

        Actions = [
            new PassAction(),
            new PlayLandSpecialAction(),
            new CastSpellAction(),
        ];
    }

    // methods

    public int[] GetWinningTeams()
    {
        if (_winningTeams is null)
            throw new Exception($"Called {nameof(GetWinningTeams)} while winning teams are not decided");
        return _winningTeams;
    }

    public Player GetActivePlayer() => Players[TurnManager.ActivePlayerIdx];


    public async Task Run()
    {
        // Game start

        // Choose the first active player
        var active = GetActivePlayer();
        var chosenActivePlayer = await active.ChoosePlayer(
            [.. Players ],
            "Choose the active player"
        );
        TurnManager.ActivePlayerIdx = chosenActivePlayer.Idx;

        // Set life totals
        foreach (var player in Players)
        {
            player.Life.SetRaw(Config.StartingLifeTotal);
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
        while (!AreWinnersDecided())
        {
            ++TurnCounter;
            
            for (
                TurnManager.ResetTurn();
                !TurnManager.IsTurnEnded() && !AreWinnersDecided();
                TurnManager.AdvancePhase()
            )
            {
                var phase = TurnManager.GetCurrentPhase();

                await phase.Do();
                if (AreWinnersDecided()) return;
            }


            TurnManager.ResetTurn();
            TurnManager.AdvanceTurn();

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
        while (!AreWinnersDecided())
        {
            CreatePriority();

            await Priority!.Resolve();

            Priority = null;

            if (Stack.IsEmpty() || AreWinnersDecided()) break;

            await Stack.ResolveTop();
        }
    }

    public void ResetPriority()
    {
        if (Priority is null)
            throw new Exception($"Called {nameof(ResetPriority)} while no priority is present!");
        Priority.Reset();
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

    public string? MoveCard(
        Card card,
        ICardZone toZone,
        CardZoneChangeType type
    )
    {
        ZoneChange = new(card, toZone, type);

        // TODO apply all zone change replacement effects

        var newId = ZoneChange.Process();
        ZoneChange = null;
        return newId;
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
            await player.Update($"Waiting for {player.GetDisplayName()}");
        }
    }

    public async Task PutOntoTheBattlefield(Card card, Player controller)
    {
        await Battlefield.MoveCard(card, controller);
    }

    public void EmptyManaPools()
    {
        foreach (var player in Players)
        {
            player.ManaPool.Clear();
        }
    }

    public void CheckForWinners()
    {
        if (AreWinnersDecided()) return;

        HashSet<int> winningTeams = [];
        HashSet<int> losingTeams = [];
        int winningTeam = -1;
        bool winnerDecided = false;

        foreach (var (tIdx, players) in _teams)
        {
            if (players.All(p => p.Status == PlayerStatus.Lost))
            {
                losingTeams.Add(tIdx);
                continue;
            }
            winningTeam = tIdx;

            if (players.Any(p => p.Status == PlayerStatus.Won))
            {
                winningTeams.Add(tIdx);
                winnerDecided = true;
            }
        }

        if (winnerDecided)
        {
            SetWinningTeams([ .. winningTeams ]);
            return;
        }

        if (losingTeams.Count == _teams.Count)
        {
            SetWinningTeams([]);
            return;
        }

        if (losingTeams.Count == _teams.Count - 1)
        {
            SetWinningTeams([ winningTeam ]);
            return;
        }
    }

    private void SetWinningTeams(int[] winningTeams)
    {
        _winningTeams = winningTeams;

        foreach (var p in Players)
        {
            p.SetStatus(
                _winningTeams.Contains(p.GetTeamIdx())
                    ? PlayerStatus.Won
                    : PlayerStatus.Lost
            );
        }
    }

    public bool AreWinnersDecided() => _winningTeams is not null;
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if a session with the specified parameters can't be created
/// </summary>
[Serializable]
public class CantStartException : Exception
{
    public CantStartException() { }
    public CantStartException(string message) : base(message) { }
    public CantStartException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if a team has more players than <c>MatchConfig.MaxTeamSize</c>
/// </summary>
[Serializable]
public class TeamTooBigException : CantStartException
{
    public TeamTooBigException() { }
    public TeamTooBigException(string message) : base(message) { }
    public TeamTooBigException(string message, System.Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if two or more players have duplicate names
/// </summary>
[Serializable]
public class DuplicatePlayerNameException : CantStartException
{
    public DuplicatePlayerNameException() { }
    public DuplicatePlayerNameException(string message) : base(message) { }
    public DuplicatePlayerNameException(string message, System.Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if the team count is bigger than <c>MatchConfig.TeamCount</c>
/// </summary>
[Serializable]
public class TooManyTeamsException : CantStartException
{
    public TooManyTeamsException() { }
    public TooManyTeamsException(string message) : base(message) { }
    public TooManyTeamsException(string message, System.Exception inner) : base(message, inner) { }
}