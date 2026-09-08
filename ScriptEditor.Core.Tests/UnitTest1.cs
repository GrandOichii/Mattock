namespace ScriptEditor.Core.Tests;

public class TODONameMe
{
    [Fact]
    public void SingleOutputTests()
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
                            Script = "Hello, world!",
                            Type = "1"
                        }
                    ],
                    SimpleArgs = []
                }
            ]
        };

        ScriptEditorState state = new()
        {
            Nodes = [
                new() {
                    Id = 0,
                    Data = [],
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
        generated.ShouldBe("Hello, world!");
    }
}