using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Matches.Scripting;
using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Stack;
using Mattock.Core.Matches.StateBasedActions;
using Mattock.Core.Matches.Turns;
using Mattock.Core.Matches.Zones;
using Mattock.Core.Setup;
using NLua;

namespace Mattock.Core.Matches;

public class Match
    : IHasSnapshot<Match.Snapshot>
{
    // properties
    public Rng Rng { get; }
    public Player[] Players { get; }
    public Battlefield Battlefield { get; }
    public MatchStack Stack { get; }
    public TurnManager TurnManager { get; }
    public int TurnCounter { get; private set; }

    public Lua LState { get; }
    public MatchConfig Config { get; }
    public Mechanics Mechanics { get; }
    private readonly Dictionary<int, Player[]> _teams;
    private int _lastCardId;
    private int _lastAAId;
    public CardZoneChange? ZoneChange { get; private set; }
    public MatchEvents Events { get; }
    public Priority? Priority { get; private set; }
    public StateBasedActionsManager StateBasedActions { get; }
    public IAction[] Actions { get; }
    public List<Card> Cards { get; }
    private int[]? _winningTeams;
    public SnapshotsManager Snapshots { get; }

    // constructors

    public Match(
        MatchConfig config,
        PlayerSetup[] playerSetups,
        Mechanics mechanics,
        string[] setupScripts
    )
    {
        Config = config;
        Mechanics = mechanics;

        ZoneChange = null;
        Priority = null;
        Stack = new(this);
        Events = new(this);
        Battlefield = new(this);
        TurnManager = new(this);
        Snapshots = new(this);
        StateBasedActions = new(this);
        _lastCardId = 0;
        _lastAAId = 0;
        TurnCounter = 0;
        Cards = [];
        _winningTeams = null;

        LState = new();
        foreach (var setupScript in setupScripts)
        {
            LState.DoString(setupScript);
        }

        var _ = new MatchScripts(this);

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
            ? new(new Random().Next())
            : new(config.Seed);
        TurnManager.ActivePlayerIdx = config.RandomFirstPlayer
            ? Rng.Next() % Players.Length
            : config.FirstPlayerIdx;

        Actions = [
            new PassAction(),
            new PlayLandSpecialAction(),
            new CastSpellAction(),
            new ActivateAbilityAction(),
            new ActivateManaAbilityAction(),
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
        var (chosenActivePlayers, rollback) = await active.ChoosePlayers(
            [.. Players ],
            1, 1,
            "Choose the active player"
        );
        if (rollback is not null)
            throw new Exception($"Player {active.GetDisplayName()} requested a rollback while choosing the first player");
        TurnManager.ActivePlayerIdx = chosenActivePlayers[0].Idx;

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

    public async Task<bool> CreateAndResolvePriority()
    {
        var effectsResolved = false;
        
        do
        {
            CreatePriority();

            await Priority!.Resolve();

            Priority = null;

            if (Stack.IsEmpty() || AreWinnersDecided()) break;

            await Stack.ResolveTop();
            effectsResolved = true;
        }
        while (!AreWinnersDecided() && !Stack.IsEmpty());

        return effectsResolved;
    }

    public void ResetPriority(int playerIdx)
    {
        if (Priority is null)
            throw new Exception($"Called {nameof(ResetPriority)} while no priority is present!");
        Priority.Reset(playerIdx);
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
                var (resp, rollback) = await f.Player.ChooseString([ "Yes", "No" ], "Mulligan?");
                if (rollback is not null)
                    throw new Exception($"Player requested rollback while deciding to mulligan"); // TODO type
                f.WillTakeMulligan = resp == "Yes";
            }

            foreach (var f in mulliganFrames)
            {
                await f.Do();
            }
        }
    }

    public string GenerateActivatedAbilityId()
    {
        return $"aa{++_lastAAId}";
    }

    public string GenerateCardId(Card card) {
        Cards.Add(card);
        return $"c{++_lastCardId}";
    }

    public Card GetCardById(string id) => Cards.Single(c => c.Id == id);

    public Card[] GetCards() => [.. Cards];

    public string? MoveCard(
        Card card,
        CardZoneChangeType type,
        ICardZoneChanger changer
    )
    {
        ZoneChange = new(card, type, changer);

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

    public async Task<string?> PutOntoTheBattlefield(Card card, Player controller)
    {
        return await Battlefield.MoveCard(card, controller);
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

    public async Task ProcessEvent(IEvent e)
    {
        await e.Do(this);
    }

    public Player[] GetPlayersInAPNAP()
    {
        List<int> result = [ TurnManager.ActivePlayerIdx ];
        while (true)
        {
            var next = TurnManager.NextInTurnOrderIdx(result.Last());
            if (next == result.First()) break;
            result.Add(next);
        }

        return [.. result.Select(idx => Players[idx])];
    }

    public async Task LoadSnapshot(Snapshot s)
    {
        throw new NotImplementedException();
    }

    public Snapshot GetSnapshot()
    {
        return new()
        {
            Rng = Rng.GetSnapshot(),
            Battlefield = Battlefield.GetSnapshot(),
            Stack = Stack.GetSnapshot(),
            TurnManager = TurnManager.GetSnapshot(),
            Players = [.. Players.Select(p => p.GetSnapshot())],

            TurnCounter = TurnCounter,
        };
    }

    public class Snapshot
    {
        public required Rng.Snapshot Rng { get; init; }
        public required Player.Snapshot[] Players { get; init; }
        public required Battlefield.Snapshot Battlefield { get; init; }
        public required MatchStack.Snapshot Stack { get; init; }
        public required TurnManager.Snapshot TurnManager { get; init; }
        public required int TurnCounter { get; init; }
    }
}
