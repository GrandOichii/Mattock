namespace ScriptEditor.Core.Tests.SimpleArgs;

public class IntegerArgTests
{
    [Theory]
    [InlineData(false, "Value: (0), end of script")]
    [InlineData(true, "Value: , end of script")]
    public void ZeroMeansEmpty_With_NoScriptIfEmpty(bool zeroMeansEmpty, string expectedScript)
    {
        // Arrange
        ScriptNodeCollection scripts = new()
        {
            Nodes = [
                new() {
                    Description = "TODO",
                    InputArray = null,
                    Inputs = [],
                    Label = "TODO",
                    Name = "TODO",
                    Outputs = [
                        new() {
                            Label = "",
                            Position = ScriptNodePortPosition.Left,
                            Script = "Value: $s, end of script",
                            Type = "1"
                        }
                    ],
                    SimpleArgs = [
                        new ScriptNodeSimpleArg() {
                            Config = new IntegerArgConfig() {
                                HasMax = false,
                                HasMin = false,
                                Max = 0,
                                Min = 0,
                                ZeroMeansEmpty = zeroMeansEmpty
                            },
                            Key = "s",
                            NoScriptIfEmpty = true,
                            Postfix = ")",
                            Prefix = "("
                        }
                    ]
                }
            ]
        };

        ScriptEditorState state = new()
        {
            Nodes = [
                new() {
                    Id = 0,
                    Data = new() {
                        { "s", 0 }
                    },
                    EditorOffsetX = 0,
                    EditorOffsetY = 0,
                    EditorSizeX = 0,
                    EditorSizeY = 0,
                    InputArray = null,
                    Name = "TODO",
                }
            ],
            Connections = [],
            StartId = 0
        };

        // Act
        var generated = state.GenerateScript(scripts);

        // Assert
        generated.ShouldBe(expectedScript);
    }
}