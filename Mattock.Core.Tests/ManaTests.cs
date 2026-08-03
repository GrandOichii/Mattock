using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Tests;

public class ManaTests
{
    private static void TestFormatted(string formatted, List<ManaAmount> expected)
    {
        // Arrange

        // Act
        var mana = ManaAmount.FromFormatted(formatted);

        // Assert
        mana.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void Formatted_W()
    {
        TestFormatted("{W}", [
            new(ManaType.White, 1)
        ]);
    }

    [Fact]
    public void Formatted_U()
    {
        TestFormatted("{U}", [
            new(ManaType.Blue, 1)
        ]);
    }

    [Fact]
    public void Formatted_B()
    {
        TestFormatted("{B}", [
            new(ManaType.Black, 1)
        ]);
    }

    [Fact]
    public void Formatted_R()
    {
        TestFormatted("{R}", [
            new(ManaType.Red, 1)
        ]);
    }

    [Fact]
    public void Formatted_G()
    {
        TestFormatted("{G}", [
            new(ManaType.Green, 1)
        ]);
    }

    [Fact]
    public void Formatted_C()
    {
        TestFormatted("{C}", [
            new(ManaType.Colorless, 1)
        ]);
    }

    [Fact]
    public void Formatted_WUBRG()
    {
        TestFormatted("{W}{U}{B}{R}{G}", [
            new(ManaType.White, 1),
            new(ManaType.Blue, 1),
            new(ManaType.Black, 1),
            new(ManaType.Red, 1),
            new(ManaType.Green, 1),
        ]);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Formatted_MultipleSame(int amount)
    {
        var mana = new ManaAmount(ManaType.White, amount);
        TestFormatted(
            string.Concat(Enumerable.Repeat("{W}", amount)),
            [ mana ]
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Formatted_Generic(int amount)
    {
        var mana = new ManaAmount(null, amount);
        TestFormatted(
            $"{{{amount}}}",
            [ mana ]
        );
    }
}