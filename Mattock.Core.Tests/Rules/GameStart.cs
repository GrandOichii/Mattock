
using Mattock.Core.Tests.Setup;

namespace Mattock.Core.Tests.Rules;

public class GameStart
{
    [Theory]
    [Trait("Rules", "103,103.1.,103.4.")]
    [InlineData(0)]
    [InlineData(1)]
    public async Task InitialValues(int firstPlayerIdx)
    {
        // Arrange
        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(firstPlayerIdx)
            .Build();

        var p2 = new TestPlayerControllerBuilder("p2")
            .Build();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ p1, p2 ]
        );

        await match.Run();

        // Assert
        match.Assert(a => a
            .ActivePlayerIs(firstPlayerIdx)
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }
}