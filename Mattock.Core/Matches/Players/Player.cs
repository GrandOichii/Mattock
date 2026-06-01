using System.Drawing;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
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
    public ManaPool ManaPool { get; }

    public Library Library { get; }
    public Hand Hand { get; }
    public Graveyard Graveyard { get; }
    public Dictionary<string, OwnedCardZone> OwnedZoneMap { get; }

    private bool _libraryFormed;

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
        Controller = setup.Controller;

        Life = new(this);
        ManaPool = new(this);
        Library = new(this);
        Hand = new(this);
        Graveyard = new(this);

        _libraryFormed = false;
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

    public bool IsActive() => Idx == Match.TurnOrderManager.ActivePlayerIdx;


    public bool IsNonActive() => Idx != Match.TurnOrderManager.ActivePlayerIdx;


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

            throw new NotImplementedException();
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
        // TODO some effects change this
        return [ 
            .. Match.Cards.Where(c => 
                c.Zone == Hand &&
                c.IsLand()
            )
        ];
    }

    public async Task<ICommand> ChooseCommand(ICommand[] options)
    {
        await Match.UpdateExcept(this);
        return await Controller.ChooseCommand(this, options);
    }

    public async Task<Card> ChooseCard(Card[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await Controller.ChooseCard(this, options, hint);
    }

    public async Task<string> ChooseString(string[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await Controller.ChooseString(this, options, hint);
    }

    public async Task<Player> ChoosePlayer(Player[] options, string hint)
    {
        await Match.UpdateExcept(this);
        return await Controller.ChoosePlayer(this, options, hint);
    }
}