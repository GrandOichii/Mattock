using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Zones;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches.Players;

/// <summary>
/// Status of the player
/// </summary>
public enum PlayerStatus
{
    /// <summary>
    /// Player is in-game and allowed to take actions
    /// </summary>
    InGame,

    /// <summary>
    /// Player lost the match and can no longer take actions
    /// </summary>
    Lost,

    /// <summary>
    /// Player won the match and can no longer take actions
    /// </summary>
    Won,
}

/// <summary>
/// One of the match players
/// </summary>
public class Player
{
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }

    /// <summary>
    /// Index of the player
    /// </summary>
    public int Idx { get; }

    /// <summary>
    /// Initial player setup data
    /// </summary>
    public PlayerSetup Setup { get; }

    /// <summary>
    /// Player choices controller
    /// </summary>
    private readonly SafePlayerControllerWrapper _controller;

    /// <summary>
    /// Life
    /// </summary>
    public Life Life { get; }

    /// <summary>
    /// Mana pool
    /// </summary>
    public ManaPool ManaPool { get; }

    /// <summary>
    /// Current player status
    /// </summary>
    public PlayerStatus Status { get; private set; }

    /// <summary>
    /// Player library
    /// </summary>
    public Library Library { get; }

    /// <summary>
    /// Player hand
    /// </summary>
    public Hand Hand { get; }

    /// <summary>
    /// Player graveyard
    /// </summary>
    public Graveyard Graveyard { get; }

    /// <summary>
    /// Dictionary of (Zone name) -> (Zone)
    /// </summary>
    public Dictionary<string, OwnedCardZone> OwnedZoneMap { get; }

    /// <summary>
    /// Is the library of the player formed?
    /// </summary>
    private bool _libraryFormed;

    /// <summary>
    /// Did the player draw from an empty library?
    /// </summary>
    public bool DrewFromEmptyLibrary { get; private set; }

    /// <summary>
    /// Amount of lands played this turn
    /// </summary>
    public int LandsPlayedThisTurn { get; set; }

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
        _controller = new SafePlayerControllerWrapper(setup.Controller);

        Life = new(this);
        ManaPool = new(this);
        Library = new(this);
        Hand = new(this);
        Graveyard = new(this);
        Status = PlayerStatus.InGame;

        _libraryFormed = false;
        DrewFromEmptyLibrary = false;
        LandsPlayedThisTurn = 0;
        OwnedZoneMap = new()
        {
            { Hand.GetZoneName(), Hand },
            { Library.GetZoneName(), Library },
            { Graveyard.GetZoneName(), Graveyard },
        };
    }

    // methods

    /// <summary>
    /// Get the team index
    /// </summary>
    /// <returns>Team index</returns>
    public int GetTeamIdx() => Setup.TeamIdx;

    /// <summary>
    /// Set the player status
    /// </summary>
    /// <param name="status">New status</param>
    /// <param name="silent">If true, check for winners is skipped</param>
    public void SetStatus(PlayerStatus status, bool silent=false)
    {
        Status = status;
        if (silent) return;
        Match.CheckForWinners();
    }

    /// <summary>
    /// Get the zone by it's name
    /// </summary>
    /// <param name="zoneName">Zone name</param>
    /// <returns>Corresponding zone</returns>
    public OwnedCardZone GetZoneByName(string zoneName) => OwnedZoneMap[zoneName];

    /// <summary>
    /// Reset all the end of turn trackers
    /// </summary>
    public void ResetTrackers()
    {
        LandsPlayedThisTurn = 0;
    } 

    /// <summary>
    /// Is the player the active player
    /// </summary>
    /// <returns>True if the player is active, otherwise false</returns>
    public bool IsActive() => Idx == Match.TurnManager.ActivePlayerIdx;

    /// <summary>
    /// Is the player not the active player
    /// </summary>
    /// <returns>True if the player is not active, otherwise false</returns>
    public bool IsNonActive() => !IsActive();

    /// <summary>
    /// Get the display name of the player (for logging)
    /// </summary>
    /// <returns>Display name</returns>
    public string GetDisplayName() => $"{Setup.Name} [{Idx}]";

    /// <summary>
    /// Form the library of the player (should only be called once)
    /// </summary>
    /// <exception cref="Exception">TODO</exception>
    public void FormLibrary()
    {
        if (_libraryFormed)
            throw new Exception($"Called {nameof(FormLibrary)} on player {GetDisplayName()}, whose library is already formed");
        _libraryFormed = true;

        foreach (var insert in Setup.Deck.MainDeck)
        {
            for (int i = 0; i < insert.Amount; ++i)
            {
                var card = new Card(this, insert.Card);
                Library.AddRaw(card);
            }
        }

        Library.Shuffle();
    }

    /// <summary>
    /// Draw the specified amount of cards
    /// </summary>
    /// <param name="amount">Number of cards to be drawn</param>
    public void Draw(int amount)
    {
        GameEndSafeguard();

        for (; amount > 0; --amount)
        {
            DrawSingle();
        }
    }

    /// <summary>
    /// Draw a single card from the library
    /// </summary>
    public void DrawSingle()
    {
        var card = Library.GetLast();
        if (card is null)
        {
            if (!Match.Config.GameLossIfRequiredToDrawFromEmptyLibrary)
                return;

            DrewFromEmptyLibrary = true;
            return;
        }

        Match.MoveCard(
            card,
            CardZoneChangeType.Bottom,
            Hand.GetCardZoneChanger()
        );
    }

    /// <summary>
    /// Shuffle the hand into the library
    /// </summary>
    public void ShuffleHandIntoLibrary()
    {
        for (var last = Hand.GetLast(); last is not null; last = Hand.GetLast())
        {
            Match.MoveCard(
                last,
                CardZoneChangeType.Bottom,
                Library.GetCardZoneChanger()
            );
        }
        Library.Shuffle();
    }

    /// <summary>
    /// Prompt the controller for a command
    /// </summary>
    /// <returns>Command</returns>
    public async Task<ICommand> PromptCommand()
    {
        List<ICommand> available = Match.GetAvailableCommands(this);

        return await ChooseCommand([.. available]);
    }

    /// <summary>
    /// Get the maximum hand size of the player
    /// </summary>
    /// <returns>Max hand size</returns>
    public int? GetMaxHandSize()
    {
        // TODO
        return Match.Config.MaxHandSize;
    }

    /// <summary>
    /// Get the maximum amount of playable lands per turn
    /// </summary>
    /// <returns></returns>
    public int? GetMaxLandsPerTurn()
    {
        // TODO
        return Match.Config.MaxLandsPerTurn;
    }

    /// <summary>
    /// Get the cards that can be played as lands
    /// </summary>
    /// <returns>Cards that can be played as lands</returns>
    public Card[] GetPlayableLands()
    {
        return [ 
            .. Match.Cards.Where(c => 
                c.CanBePlayedAsLand(this)
            )
        ];
    }

    /// <summary>
    /// Get the cards that can be cast
    /// </summary>
    /// <returns>Castable cards</returns>
    public Card[] GetCastableCards()
    {
        return [.. Match.Cards.Where(c => 
            c.CanBeCast(this)
        )];
    }

    /// <summary>
    /// Cast the card
    /// </summary>
    /// <param name="card">Castable card</param>
    /// <exception cref="Exception">TODO</exception>
    public async Task Cast(Card card)
    {
        await Match.Events.CastSpell(this, card);
    }

    // TODO remove the Card, costs can be payed for many other things
    // TODO docs
    public async Task PayCost(Card card, CostCollection cost)
    {
        // mana
        var manaCosts = cost.GetManaCosts();
        while (manaCosts.Count > 0)
        {
            var manaCost = manaCosts.Dequeue();
            for (int i = 0; i < manaCost.Amount; ++i)
            {
                var candidates = ManaPool.GetCandidates(manaCost.Type);
                if (candidates.Count == 0)
                {
                    var postFix = manaCost.Type is null
                        ? "generic type"
                        : $"type {manaCost.Type}";
                    throw new Exception($"Code error: failed to find stored mana candidates to pay for mana cost of {postFix}");
                }
                var choice = await ChooseStoredMana([.. candidates], $"Pay for card {card.GetDisplayName()}");
                ManaPool.Remove(choice);
            }
        }

        // other
        // TODO
    }

    /// <summary>
    /// Is the player still in game
    /// </summary>
    /// <returns>True if the player is in game, otherwise false</returns>
    public bool IsInGame() => Status == PlayerStatus.InGame;

    /// <summary>
    /// A safeguard that checks whether the player is still in game
    /// </summary>
    /// <exception cref="Exception">TODO</exception>
    private void GameEndSafeguard()
    {
        if (IsInGame()) return;

        throw new Exception($"Code error: tried to prompt a choice from player {GetDisplayName()} while their status is {Status}");
    }

    /// <summary>
    /// Is the player an opponent for the specified player
    /// </summary>
    /// <param name="player">True if the players are opponents, otherwise false</param>
    /// <returns></returns>
    public bool IsOpponentFor(Player player) => GetTeamIdx() != player.GetTeamIdx();

    /// <summary>
    /// Is the player an ally for the specified player
    /// </summary>
    /// <param name="player">True if the players are teammates, otherwise false</param>
    /// <returns></returns>
    public bool IsTeammateFor(Player player) => GetTeamIdx() == player.GetTeamIdx();

    /// <summary>
    /// Discard the specified cards
    /// </summary>
    /// <param name="cards">Cards to be discarded</param>
    public void Discard(Card[] cards)
    {
        foreach (var card in cards)
        {
            Match.MoveCard(
                card,
                CardZoneChangeType.Top, // TODO
                Graveyard.GetCardZoneChanger()
            );
        }
    }

    /// <summary>
    /// Get the available attack declarations for the combat step
    /// </summary>
    /// <returns>Available attack declarations</returns>
    public AttackDeclaration[] GetAvailableAttackDeclarations()
    {
        List<AttackDeclaration> result = [];

        var permanents = Match.Battlefield.GetPermanentsControlledBy(this);
        foreach (var permanent in permanents)
        {
            result.AddRange(permanent.GetAvailableAttackDeclarations());
        }

        return [.. result];
    }

    /// <summary>
    /// Get the available block declarations for the combat step
    /// </summary>
    /// <returns>Available block declarations</returns>
    public BlockDeclaration[] GetAvailableBlockDeclarations()
    {
        Permanent[] attackers = [.. Match
            .Battlefield
            .GetAttackingPermanents(this)];

        return [.. Match
            .Battlefield
            .GetPermanents()
            .SelectMany(p => p.GetAvailableBlockDeclarations(this, attackers))
        ];
    }

    /// <summary>
    /// Update the player controller with the actual game state
    /// </summary>
    /// <param name="msg">State message</param>
    public async Task Update(string msg)
    {
        await _controller.Update(this, msg);
    }

    /// <summary>
    /// Choose a command
    /// </summary>
    /// <param name="options">Available commands</param>
    /// <returns>Chosen command</returns>
    public async Task<ICommand> ChooseCommand(ICommand[] options)
    {
        GameEndSafeguard();
        await Match.UpdateExcept(this);
        return await _controller.ChooseCommand(this, options);
    }

    // TODO docs
    public async Task<Card?> ChooseCard(Card[] options, string hint, bool allowNone)
    {
        GameEndSafeguard();
        await Match.UpdateExcept(this);
        var result = await _controller.ChooseCard(this, options, hint, allowNone);

        if (result is null && !allowNone)
            throw new Exception($"Controller provided null card for {nameof(ChooseCard)} where {nameof(allowNone)} = false"); // TODO type

        return result;
    }

    /// <summary>
    /// Choose a string
    /// </summary>
    /// <param name="options"></param>
    /// <param name="hint"></param>
    /// <returns></returns>
    public async Task<string> ChooseString(string[] options, string hint)
    {
        GameEndSafeguard();
        await Match.UpdateExcept(this);
        var result = await _controller.ChooseString(this, options, hint, false);

        return result!;
    }

    public async Task<Player[]> ChoosePlayers(Player[] options, int min, int max, string hint)
    {
        GameEndSafeguard();
        if (max == 0)
            throw new Exception($"Provided max = 0 for {nameof(ChoosePlayers)}"); // TODO type
            
        await Match.UpdateExcept(this);
        var result = await _controller.ChoosePlayers(this, options, min, max, hint);

        return result!;
    }

    public async Task<StoredMana> ChooseStoredMana(StoredMana[] options, string hint)
    {
        GameEndSafeguard();
        await Match.UpdateExcept(this);
        var result = await _controller.ChooseStoredMana(this, options, hint, false)!;

        return result!;
    }

    public async Task<CostCollection> ChooseCostCollection(CostCollection[] options, string hint)
    {
        GameEndSafeguard();
        if (options.Length == 0)
            throw new Exception($"Provided empty options for {nameof(ChooseCostCollection)} (hint: {hint})");
        if (options.Length == 1)
            return options[0];

        var result = await _controller.ChooseCostCollection(this, options, hint, false);

        return result!;
    }

    public async Task<AttackDeclaration[]> ChooseAttackDeclarations(AttackDeclaration[] options)
    {
        GameEndSafeguard();
        if (options.Length == 0)
            throw new Exception($"Provided empty options for {nameof(ChooseAttackDeclarations)}");
        var result = await _controller.ChooseAttackDeclarations(this, options);

        return result;        
    }

    public async Task<BlockDeclaration[]> ChooseBlockDeclarations(BlockDeclaration[] options)
    {
        GameEndSafeguard();

        if (options.Length == 0)
            throw new Exception($"Provided empty options for {nameof(ChooseBlockDeclarations)}"); // TODO type
        var result = await _controller.ChooseBlockDeclarations(this, options);

        return result;
    }
}