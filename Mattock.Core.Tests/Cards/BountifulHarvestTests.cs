using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Bountiful Harvest
/// </summary>
public class BountifulHarvestTests
{
    private static void HasLife(int pIdx, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .HasLife(expected)
            )
        );
    }

    [Fact]
    public async Task NoLands()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ new() {
                Amount = 7,
                Card = card,
            } ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task SingleLand()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 6,
                    Card = card,
                },
                new DeckCardTemplateBuilder("land")
                    .Land()
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.PlayLandWithName("land")
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 21, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task SingleOpponentLand()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(1)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 6,
                    Card = card,
                },
                new DeckCardTemplateBuilder("land")
                    .Land()
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .ChoosePlayers.WithIdx(1)
            .SetDeck(deck)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.PlayLandWithName("land")
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task SingleNonLand()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 6,
                    Card = card,
                },
                new DeckCardTemplateBuilder("artifact")
                    .ZeroCost()
                    .Artifact()
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.CastSpellWithName("artifact")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task DoubleLand()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxLandsPerTurn()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 5,
                    Card = card,
                },
                new DeckCardTemplateBuilder("land")
                    .Land()
                    .Amount(2)
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.PlayLandWithName("land")
            .Act.PlayLandWithName("land")
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 22, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(22)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task LandX5()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Bountiful Harvest");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxLandsPerTurn()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 2,
                    Card = card,
                },
                new DeckCardTemplateBuilder("land")
                    .Land()
                    .Amount(5)
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.PlayLandWithName("land")
            .Act.PlayLandWithName("land")
            .Act.PlayLandWithName("land")
            .Act.PlayLandWithName("land")
            .Act.PlayLandWithName("land")
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 25, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .HasLife(25)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }
}