using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Naturalize
/// </summary>
public class NaturalizeTests
{
    [Fact]
    public async Task NoTargets()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Naturalize");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .CantCastSpell()
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
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
        );
    }

    [Fact]
    public async Task CantTargetCreature()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Naturalize");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("creature")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/1")
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("creature")
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CantCastSpell()
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
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
        );
    }

    [Theory]
    [InlineData("artifact")]
    [InlineData("enchantment")]
    public async Task CanTarget(string cardName)
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Naturalize");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("artifact")
                    .Amount(2)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("enchantment")
                    .Amount(2)
                    .Enchantment()
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("creature")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/1")
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Green, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(cardName)
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CanCastSpell()
            )
            .Act.CastSpellWithName(card.Name)
            .ChoosePermanents.Assert(a => a
                .OptionsCount(1)
            )
            .ChoosePermanents.First()
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .AssertAsSpell(asp => asp
                                .CardName(card.Name)
                            )
                            // TODO check that is targeting
                        )
                    )
                )
            )
            .Act.Pass()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
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
            .AssertBattlefield(ab => ab.IsEmpty())
            .AssertPlayer(0, ap => ap
                .AssertGraveyard(ag => ag.HasCardCount(2))
            )
        );
    }
}