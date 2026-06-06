using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Tests.Rules.Costs;

// 118.2.
// 601.2f
// 601.2h

public class ManaCostTests
{
    [Theory]
    [Trait("Rules", "118.2.,601.2f,601.2h")]
    // *** cant pay
    // * no cost
    [InlineData(false, "", "{W}{U}{B}{R}{G}{C}")]
    // * colored
    [InlineData(false, "{W}", "")]
    [InlineData(false, "{U}", "")]
    [InlineData(false, "{B}", "")]
    [InlineData(false, "{R}", "")]
    [InlineData(false, "{G}", "")]
    [InlineData(false, "{C}", "")]
    [InlineData(false, "{W}", "{U}")]
    [InlineData(false, "{W}", "{B}")]
    [InlineData(false, "{W}", "{R}")]
    [InlineData(false, "{W}", "{G}")]
    [InlineData(false, "{W}", "{C}")]
    [InlineData(false, "{W}{W}", "{W}")]
    [InlineData(false, "{W}{W}", "{W}{C}")]
    // * generic
    [InlineData(false, "{1}", "")]
    [InlineData(false, "{2}", "{W}")]
    [InlineData(false, "{3}", "{W}{C}")]
    // *** can pay
    // * zero cost
    [InlineData(true, "{0}", "")]
    // * generic mana
    [InlineData(true, "{1}", "{W}")]
    [InlineData(true, "{1}", "{U}")]
    [InlineData(true, "{1}", "{B}")]
    [InlineData(true, "{1}", "{R}")]
    [InlineData(true, "{1}", "{G}")]
    [InlineData(true, "{1}", "{C}")]
    // * colored mana
    [InlineData(true, "{W}", "{W}")]
    [InlineData(true, "{U}", "{U}")]
    [InlineData(true, "{B}", "{B}")]
    [InlineData(true, "{R}", "{R}")]
    [InlineData(true, "{G}", "{G}")]
    [InlineData(true, "{C}", "{C}")]
    [InlineData(true, "{W}{U}{B}{R}{G}{C}", "{W}{U}{B}{R}{G}{C}")]
    // * generic and colored
    [InlineData(true, "{1}{W}{U}", "{W}{W}{U}")]
    [InlineData(true, "{1}{W}{U}", "{U}{W}{U}")]
    [InlineData(true, "{1}{W}{U}", "{B}{W}{U}")]
    [InlineData(true, "{1}{W}{U}", "{R}{W}{U}")]
    [InlineData(true, "{1}{W}{U}", "{G}{W}{U}")]
    [InlineData(true, "{1}{W}{U}", "{C}{W}{U}")]
    public async Task ManaPaymentAvailability(bool canPay, string formattedCost, string formattedMana)
    {
        // Arrange
        var card = new DeckCardTemplateBuilder("i1")
            .ManaCost(ManaCost.FromFormattedCost(formattedCost))
            .Instant();
        var mana = StoredMana.FromFormattedMana(formattedMana);
        var deck = new DeckTemplate()
        {
            MainDeck = [
                card.Build()
            ]
        };

        Action<CommandChoicesBuilder.Asserts> assert = canPay
            ? a => a.CanCastSpell()
            : a => a.CantCastSpell();

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.ForEach(mana, (m, a) => a
                .AddMana(m.Type, 1)
            )
            .Act.Pass()
            .Act.Assert(assert)
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .DisableManaPoolEmptying()
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }

    // [Fact]
    // [Trait("Rules", "118.2.,601.2f,601.2h")]
    // public async Task ManaPayment()
    // {
    //     // Arrange
    //     var card = new DeckCardTemplateBuilder("i1")
    //         .AddManaCost(new()
    //         {
    //             Type = null,
    //             Amount = 1
    //         })
    //         .Instant();
    //     var deck = new DeckTemplate()
    //     {
    //         MainDeck = [
    //             card.Build() 
    //         ]
    //     };

    //     var p1 = new TestPlayerControllerBuilder("p1", 0)
    //         .ChoosePlayer.WithIdx(0)
    //         .SetDeck(deck)
    //         // *** turn 1
    //         .Act.AddMana(ManaType.White, 1)
    //         .Act.AddMana(ManaType.Blue, 1)
    //         .Act.AddMana(ManaType.Black, 1)
    //         .Act.AddMana(ManaType.Red, 1)
    //         .Act.AddMana(ManaType.Green, 1)
    //         .Act.AddMana(ManaType.Colorless, 1)
    //         .Act.Pass()
    //         .Act.Assert(a => a
    //             .CanCastSpell()
    //         )
    //         .Act.CastSpellWithName("i1")
    //         .ChooseMana.Assert(cma => cma
    //             .OptionsCount(6)
    //         )
    //         .ChooseMana.FirstOfType(ManaType.White)
    //         .Act.Pass()
    //         .Act.Crash()
    //         ;

    //     var p2 = new TestPlayerControllerBuilder("p2", 1)
    //         .SetDeck(deck)
    //         .Act.AutoPass()
    //         ;

    //     // Act
    //     var match = new TestMatchWrapper(
    //         new MatchConfigBuilder()
    //             .FirstPlayerIdx(0)
    //             .GameLossIfRequiredToDrawFromEmptyLibrary(false)
    //             .FirstPlayerNoDrawIfSingleOpponent(false)
    //             .DisableManaPoolEmptying()
    //             .Build(),
    //         [ p1, p2 ]
    //     );
    //     match.RemoveMulligans();

    //     await match.Run();

    //     // Assert
    //     match.Assert(a => a
    //         .CrashedIntentially()
    //         .NoChoicesLeft()
    //         .AssertStack(sa => sa.IsEmpty())
    //         .AssertPlayer(0, pa => pa
    //             .AssertHand(ha => ha.IsEmpty())
    //             .AssertLibrary(la => la.IsEmpty())
    //             .AssertGraveyard(ga => ga
    //                 .HasCardCount(1)
    //                 .AssertCard(0, ca => ca
    //                     .HasName("i1")
    //                 )
    //             )
    //             .AssertManaPool(mpa => mpa
    //                 .HasTotalMana(5)
    //             )
    //         )
    //     );
    // }

    [Theory]
    [InlineData("{0}", "", 0)]
    [InlineData("{W}", "{W}", 0)]
    [InlineData("{U}", "{U}", 0)]
    [InlineData("{B}", "{B}", 0)]
    [InlineData("{R}", "{R}", 0)]
    [InlineData("{G}", "{G}", 0)]
    [InlineData("{1}", "{W}", 0)]
    [InlineData("{W}{U}", "{W}{U}", 0)]
    [InlineData("{1}{U}", "{W}{U}", 0)]
    [InlineData("{0}", "{W}{U}{B}{R}{G}{C}", 6)]
    [InlineData("{1}", "{W}{U}{B}{R}{G}{C}", 5)]
    [InlineData("{2}", "{W}{U}{B}{R}{G}{C}", 4)]
    [InlineData("{3}", "{W}{U}{B}{R}{G}{C}", 3)]
    [InlineData("{4}", "{W}{U}{B}{R}{G}{C}", 2)]
    [InlineData("{5}", "{W}{U}{B}{R}{G}{C}", 1)]
    [InlineData("{6}", "{W}{U}{B}{R}{G}{C}", 0)]
    public async Task CheckAmountOfManaLeftAfterPaying(string formattedCost, string formattedMana, int manaLeft)
    {
        // Arrange
        var card = new DeckCardTemplateBuilder("i1")
            .ManaCost(ManaCost.FromFormattedCost(formattedCost))
            .Instant()
            .Build();
        var mana = StoredMana.FromFormattedMana(formattedMana);
        var deck = new DeckTemplate()
        {
            MainDeck = [
                card 
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.ForEach(mana, (m, a) => a
                .AddMana(m.Type, 1)
            )
            .Act.Pass()
            .Act.Assert(a => a
                .CanCastSpell()
            )
            .Act.CastSpellWithName("i1")
            .ChooseMana.NTimes(card.Card.GetManaValue(), cm => cm.First())
            .Act.Pass()
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .DisableManaPoolEmptying()
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertStack(sa => sa.IsEmpty())
            .AssertPlayer(0, pa => pa
                .AssertHand(ha => ha.IsEmpty())
                .AssertLibrary(la => la.IsEmpty())
                .AssertGraveyard(ga => ga
                    .HasCardCount(1)
                    .AssertCard(0, ca => ca
                        .HasName("i1")
                    )
                )
                .AssertManaPool(mpa => mpa.HasTotalMana(manaLeft))
            )
        );
    }
}