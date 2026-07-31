// using Mattock.Core.Loaders;

// namespace Mattock.Core.Tests.Cards;

// /// <summary>
// /// Tests for the card Sign in Blood
// /// </summary>
// public class SignInBloodTests
// {
//     private static void HandCardCount(int pIdx, int expectedCount, CommandChoicesBuilder.Asserts a)
//     {
//         a.AssertMatch(am => am
//             .AssertPlayer(pIdx, ap => ap
//                 .AssertHand(ah => ah
//                     .HasCardCount(expectedCount)
//                 )
//             )
//         );
//     }

//     private static void HasLife(int pIdx, int expected, CommandChoicesBuilder.Asserts a)
//     {
//         a.AssertMatch(am => am
//             .AssertPlayer(pIdx, ap => ap
//                 .HasLife(expected)
//             )
//         );
//     }

//     [Fact]
//     public async Task TargetSelf()
//     {
//         // Arrange
//         var loader = new FileCardLoader("../../../../cards");

//         var card = loader.Load("M10:Sign in Blood");
        
//         var config = new MatchConfigBuilder()
//             .FirstPlayerIdx(0)
//             .NoManaPoolEmptying()
//             .Build();
        
//         var deck = new DeckTemplate()
//         {
//             MainDeck = [ new() {
//                 Amount = 60,
//                 Card = card,
//             } ]
//         };

//         var p1 = new TestPlayerControllerBuilder("p1", 0)
//             .SetDeck(deck)
//             .ChoosePlayer.WithIdx(0)
//             .Act.AddMana(ManaType.Black, 2)
//             .Act.AutoPassToPhase(PhaseType.PrecombatMain)
//             .Act.Assert(a => HandCardCount(0, 7, a))
//             .Act.Assert(a => HasLife(0, 20, a))
//             .Act.CastSpellWithName(card.Name)
//             // .ChoosePlayer.Me()
//             // .StoredManaChoices.NTimes(2, smc => smc.First())
//             // .Act.Assert(a => HandCardCount(0, 6, a))
//             // .Act.Assert(a => HasLife(0, 20, a))
//             // .Act.AutoPassUntilStackEmpty()
//             // .Act.Assert(a => HandCardCount(0, 8, a))
//             // .Act.Assert(a => HasLife(0, 18, a))
//             // .Act.Crash()
//         ;

//         var p2 = new TestPlayerControllerBuilder("p2", 1)
//             .SetDeck(deck)
//             .Act.AutoPass();

//         var match = new TestMatchWrapper(
//             config,
//             [ p1, p2 ]
//         );
//         match.RemoveMulligans();

//         // Act
//         await match.Run();

//         // Assert
//         match.Assert(a => a
//             .CrashedIntentially()
//             .NoChoicesLeft()
//             // .AssertPlayer(0, ap => ap
//             //     .AssertHand(ah => ah.HasCardCount(8))
//             //     .HasLife(18)
//             // )
//             // .AssertPlayer(1, ap => ap
//             //     .AssertHand(ah => ah.HasCardCount(7))
//             //     .HasLife(20)
//             // )
//         );
//     }
// }