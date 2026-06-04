using System.Drawing;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches.Players;

public class Player
{
    // properties

    public Match Match { get; }
    public int Idx { get; }
    public PlayerSetup Setup { get; }
    private readonly IPlayerController _controller;
    public Life Life { get; }
    public ManaPool ManaPool { get; }

    public Library Library { get; }
    public Hand Hand { get; }
    public Graveyard Graveyard { get; }
    public Dictionary<string, OwnedCardZone> OwnedZoneMap { get; }

    private bool _libraryFormed;
    public bool DrewFromEmptyLibrary { get; private set; }

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

    public OwnedCardZone GetZoneByName(string zoneName) => OwnedZoneMap[zoneName];

    public void ResetTrackers()
    {
        LandsPlayedThisTurn = 0;
    } 

    public bool IsActive() => Idx == Match.TurnManager.ActivePlayerIdx;


    public bool IsNonActive() => Idx != Match.TurnManager.ActivePlayerIdx;


    public string GetDisplayName() => $"{Setup.Name} [{Idx}]";


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

    public void Draw(int amount)
    {
        for (; amount > 0; --amount)
        {
            DrawSingle();
        }
    }

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
            Hand,
            CardZoneChangeType.Bottom
        );
    }

    public void ShuffleHandIntoLibrary()
    {
        for (var last = Hand.GetLast(); last is not null; last = Hand.GetLast())
        {
            Match.MoveCard(
                last,
                Library,
                CardZoneChangeType.Bottom
            );
        }
        Library.Shuffle();
    }

    public async Task<ICommand> PromptCommand()
    {
        List<ICommand> available = Match.GetAvailableCommands(this);

        return await ChooseCommand([.. available]);
    }

    public int? GetMaxHandSize()
    {
        // TODO
        return Match.Config.MaxHandSize;
    }

    public int? GetMaxLandsPerTurn()
    {
        // TODO
        return Match.Config.MaxLandsPerTurn;
    }

    public List<Card> GetPlayableLands()
    {
        return [ 
            .. Match.Cards.Where(c => 
                c.CanBePlayedAsLand(this)
            )
        ];
    }

    public List<Card> GetCastableCards()
    {
        return [
            .. Match.Cards.Where(c => 
                c.CanBeCast(this)
            )
        ];
    }

    public async Task Cast(Card card)
    {
        // 601.2a Move the card onto the stack
        var effect = Match.Stack.Create(card, this);
        // TODO

        // 601.2b Modal spells
        // TODO

        // 601.2c Choose targets
        // TODO
        
        // 601.2d Announce divisions
        // TODO

        // 601.2e Check if the spell can be legally cast (733)
        // TODO

        // 601.2f Determine the spell cost
        var costVariations = card.GetCostCollections(this);
        if (costVariations.Count != new HashSet<string>(costVariations.Select(c => c.Text)).Count)
        {
            throw new Exception($"Computed cost variations with duplicate texts (texts: {string.Join(", ", costVariations.Select(c => $"\"{c.Text}\""))})");
        }
        var choice = await ChooseCostCollection([.. costVariations], $"Choose how to pay for {card.GetDisplayName()}");

        // 601.2g Activate mana abilities to pay for costs
        // TODO

        // 601.2h Pay the cost
        await PayCost(card, choice);
        // TODO

        // 601.2i Modify characteristics
        // TODO

        // Triggers
        // TODO
    }

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

    public async Task Update(string msg)
    {
        await _controller.Update(this, msg);
    }

    public async Task<ICommand> ChooseCommand(ICommand[] options)
    {
        await Match.UpdateExcept(this);
        return await _controller.ChooseCommand(this, options);
    }

    public async Task<Card> ChooseCard(Card[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await _controller.ChooseCard(this, options, hint);
    }

    public async Task<string> ChooseString(string[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await _controller.ChooseString(this, options, hint);
    }

    public async Task<Player> ChoosePlayer(Player[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await _controller.ChoosePlayer(this, options, hint);
    }

    public async Task<StoredMana> ChooseStoredMana(StoredMana[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await _controller.ChooseStoredMana(this, options, hint);
    }

    public async Task<CostCollection> ChooseCostCollection(CostCollection[] options, string hint)
    {
        if (options.Length == 0)
            throw new Exception($"Provided empty options for {nameof(ChooseCostCollection)} (hint: {hint})");
        if (options.Length == 1)
            return options[0];

        return await _controller.ChooseCostCollection(this, options, hint);
    }
}