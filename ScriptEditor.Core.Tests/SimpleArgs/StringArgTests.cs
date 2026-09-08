namespace ScriptEditor.Core.Tests.SimpleArgs;

public class StringArgTests
{
    [Theory]
    [InlineData("", "", "Value: somevalue, end of script")]
    [InlineData("(", "", "Value: (somevalue, end of script")]
    [InlineData("", ")", "Value: somevalue), end of script")]
    [InlineData("(", ")", "Value: (somevalue), end of script")]
    public void PrefixPostfix(string prefix, string postfix, string expectedScript)
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
                            Config = new StringArgConfig() {
                                Default = "",
                                Multiline = false,
                                Placeholder = "",
                            },
                            Key = "s",
                            NoScriptIfEmpty = false,
                            Postfix = postfix,
                            Prefix = prefix
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
                        { "s", "somevalue" }
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

    [Fact]
    public void NoScriptIfEmpty()
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
                            Config = new StringArgConfig() {
                                Default = "",
                                Multiline = false,
                                Placeholder = "",
                            },
                            Key = "s",
                            NoScriptIfEmpty = true,
                            Postfix = "postfix",
                            Prefix = "prefix"
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
                        { "s", "" }
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
        generated.ShouldBe("Value: , end of script");
    }
}