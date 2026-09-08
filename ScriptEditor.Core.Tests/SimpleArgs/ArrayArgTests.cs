using Newtonsoft.Json;

namespace ScriptEditor.Core.Tests.SimpleArgs;

public class ArrayArgTests
{
     public static IEnumerable<object[]> ReplaceData => [
        [
            new StringArgConfig() {
                Default = "",
                Multiline = false,
                Placeholder = "",
            },
            new List<object>() {
                "v1",
                "v2",
                "v3"
            },
            "Values: (v1), (v2), (v3)"
        ],
        [
            new IntegerArgConfig() {
                HasMax = false,
                HasMin = false,
                Max = 0,
                Min = 0,
            },
            new List<object>() {
                1,
                3,
                2
            },
            "Values: (1), (3), (2)"
        ],
        [
            new BoolArgConfig() {
                FalseScript = "FALSE",
                Label = "",
                TrueScript = "TRUE",
            },
            new List<object>() {
                true,
                false,
                false
            },
            "Values: (TRUE), (FALSE), (FALSE)"
        ],
        [
            new EnumArgConfig()
            {
                Values = new()
                {
                    { "R", "Red" },
                    { "B", "Blue" },
                    { "Y", "Yellow" },
                    { "G", "Green" },
                }
            },
            new List<object>() {
                "R",
                "B",
                "G"
            } ,
            "Values: (Red), (Blue), (Green)"
        ],
    ];

    [Theory]
    [MemberData(nameof(ReplaceData))]
    public void Replace(SimpleArgConfig config, List<object> values, string expectedScript)
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
                            Script = "Values: $s",
                            Type = "1"
                        }
                    ],
                    SimpleArgs = [
                        new ScriptNodeArrayArg() {
                            Config = config,
                            AddButtonText = "",
                            Key = "s",
                            NoScriptIfEmpty = false,
                            Postfix = "",
                            Prefix = "",
                            ItemPostfix = ")",
                            ItemPrefix = "(",
                            Separator = ", "
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
                        { "s", values }
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