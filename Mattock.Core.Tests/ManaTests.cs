using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Tests;

public class ManaTests
{
    private static void TestFormatted(string formatted, List<Mana> expected)
    {
        // Arrange

        // Act
        var mana = Mana.FromFormatted(formatted);

        // Assert
        mana.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void Formatted_W()
    {
        TestFormatted("{W}", [
            new() {
                Amount = 1,
                Type = ManaType.White
            }
        ]);
    }

    [Fact]
    public void Formatted_U()
    {
        TestFormatted("{U}", [
            new() {
                Amount = 1,
                Type = ManaType.Blue
            }
        ]);
    }

    [Fact]
    public void Formatted_B()
    {
        TestFormatted("{B}", [
            new() {
                Amount = 1,
                Type = ManaType.Black
            }
        ]);
    }

    [Fact]
    public void Formatted_R()
    {
        TestFormatted("{R}", [
            new() {
                Amount = 1,
                Type = ManaType.Red
            }
        ]);
    }

    [Fact]
    public void Formatted_G()
    {
        TestFormatted("{G}", [
            new() {
                Amount = 1,
                Type = ManaType.Green
            }
        ]);
    }

    [Fact]
    public void Formatted_C()
    {
        TestFormatted("{C}", [
            new() {
                Amount = 1,
                Type = ManaType.Colorless
            }
        ]);
    }

    [Fact]
    public void Formatted_WUBRG()
    {
        TestFormatted("{W}{U}{B}{R}{G}", [
            new() {
                Amount = 1,
                Type = ManaType.White
            },
            new() {
                Amount = 1,
                Type = ManaType.Blue
            },
            new() {
                Amount = 1,
                Type = ManaType.Black
            },
            new() {
                Amount = 1,
                Type = ManaType.Red
            },
            new() {
                Amount = 1,
                Type = ManaType.Green
            },
        ]);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Formatted_MultipleSame(int amount)
    {
        var mana = new Mana()
        {
            Type = ManaType.White,
            Amount = amount,
        };
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
        var mana = new Mana() {
            Type = null,
            Amount = amount,
        };
        TestFormatted(
            $"{{{amount}}}",
            [ mana ]
        );
    }
}