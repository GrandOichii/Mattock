using System.Linq;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.Actions;

public class CastSpellActionTests
{
    public static IEnumerable<TheoryDataRow<string>> CastableTypesAll_Data => 
        CardTypes.Castable.Select(t => new TheoryDataRow<string>(t));

    public static IEnumerable<TheoryDataRow<string>> CastableTypes_SorcerySpeed_Data => 
        CardTypes.Castable.Where(t => t != CardTypes.Instant).Select(t => new TheoryDataRow<string>(t));

    public static IEnumerable<TheoryDataRow<string>> PermanentSpellTypes_Data => 
        CardTypes.Permanents.Where(t => t != CardTypes.Land).Select(t => new TheoryDataRow<string>(t));

    public static IEnumerable<TheoryDataRow<string>> NonPermanentSpellTypes_Data => 
        [ new(CardTypes.Sorcery), new(CardTypes.Instant) ];

    [Trait("Rules", "202.1b")]
    [Theory]
    [MemberData(nameof(CastableTypesAll_Data))]
    public async Task CantCastWithNoCost(string type)
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder()
                    .Amount(deckSize)
                    .AddType(type)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 2
            // * upkeep 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 2
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * beginning of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 3
            // * upkeep 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 3
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * beginning of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            // TODO declare attackers
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(3)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }

    [Trait("Rules", "300.2a")]
    [Theory]
    [MemberData(nameof(CastableTypesAll_Data))]
    public async Task CantCastLand(string type)
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder()
                    .Amount(deckSize)
                    .AddType(type)
                    .Land()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
                .CanPlayLand()
            )
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
                .CanPlayLand()
            )
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 2
            // * upkeep 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 2
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * beginning of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 3
            // * upkeep 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 3
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
                .CanPlayLand()
            )
            .Act.Pass()
            // * beginning of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
                .CanPlayLand()
            )
            .Act.Pass()
            // * end 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            // TODO declare attackers
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(3)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }

    [Trait("Rules", "117.1a")]
    [Theory]
    [MemberData(nameof(CastableTypes_SorcerySpeed_Data))]
    public async Task CanCastZeroCost_AtSorcerySpeed(string type)
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .AddType(type)
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 2
            // * upkeep 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 2
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * beginning of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * end 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // *** turn 3
            // * upkeep 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * draw 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * precombat main phase 3
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * beginning of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * declare attackers 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Pass()
            // * postcombat main 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantCastSpell()
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            // TODO declare attackers
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(3)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }

    [Trait("Rules", "117.1a,304.1.,")]
    [Fact]
    public async Task CanCastZeroCost_AtInstantSpeed()
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .Instant()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // *** turn 2
            // * upkeep 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * draw 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * precombat main phase 2
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * beginning of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * declare attackers 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * postcombat main 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // *** turn 3
            // * upkeep 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * draw 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * precombat main phase 3
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * beginning of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * declare attackers 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * postcombat main 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Pass()
            // * end 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            // TODO declare attackers
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(3)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }

    [Trait("Rules", "608.3.")]
    [Theory]
    [MemberData(nameof(PermanentSpellTypes_Data))]
    public async Task ZeroCost_Permanent(string type)
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .AddType(type)
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.CastSpellWithName("c1")
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                    .AssertStack(sa => sa
                        .EffectCount(1)
                        .AssertEffect(0, ea => ea
                            .HasController(0)
                            .AssertAsSpell(sea => sea
                                .CardName("c1")
                            )
                        )
                    )
                )
            )
            .Act.Pass() // pass priority after spell
            .Act.Pass() // pass from empty stack
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                    .AssertStack(sa => sa
                        .IsEmpty()
                    )
                )
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .AssertStack(sa => sa
                        .EffectCount(1)
                    )
                )
            )
            .Act.Pass() // pass priority after spell
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(2)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertStack(sa => sa
                .IsEmpty()
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(1)
                .AssertPermanent("c1", pa => pa
                    .ControlledBy(0)
                    .IsUntapped()
                    .IsOfType(type)
                )
            )
        );
    }

    
    [Trait("Rules", "608.2.")]
    [Theory]
    [MemberData(nameof(NonPermanentSpellTypes_Data))]
    public async Task ZeroCost_NonPermanent(string type)
    {
        // Arrange
        var deckSize = 3;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .AddType(type)
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanCastSpell()
                .OptionsCount(1 + deckSize) // pass + 3 spells
            )
            .Act.CastSpellWithName("c1")
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                    .AssertStack(sa => sa
                        .EffectCount(1)
                        .AssertEffect(0, ea => ea
                            .HasController(0)
                            .AssertAsSpell(sea => sea
                                .CardName("c1")
                            )
                        )
                    )
                )
            )
            .Act.Pass() // pass priority after spell
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                    .AssertStack(sa => sa
                        .IsEmpty()
                    )
                )
            )
            .Act.Pass() // pass from empty stack
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                    .AssertStack(sa => sa
                        .IsEmpty()
                    )
                )
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .AssertStack(sa => sa
                        .EffectCount(1)
                    )
                )
            )
            .Act.Pass() // pass priority after spell
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                    .AssertStack(sa => sa
                        .IsEmpty()
                    )
                )
            )
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(2)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(1)
                    .AssertCard(0, ca => ca
                        .HasName("c1")
                    )
                )
            )
            .AssertStack(sa => sa
                .IsEmpty()
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }


}