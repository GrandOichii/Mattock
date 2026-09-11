using ScriptEditor.Core;
using ScriptEditor.Core.SimpleArgs;

namespace Mattock.Core.Scripts;

public static class Scripts
{

    // public static ScriptNode SingleFromMemory(
    //     string labelType,
    //     string type,
    //     string table
    // )
    // {
    //     return new()
    //     {
    //         Name = $"FromMemory_{type}Single",
    //         Description = "TODO",
    //         InputArray = null,
    //         Inputs = [],
    //         Label = $"{labelType} from memory",
    //         SimpleArgs = [
    //             new ScriptNodeSimpleArg() {
    //                 Config = new StringArgConfig() {
    //                     Default = "",
    //                     Multiline = false,
    //                     Placeholder = "Enter memory key",
    //                 },
    //                 Postfix = "",
    //                 Prefix = "",
    //                 Key = "memKey",
    //             }
    //         ],
    //         Outputs = [
    //             new() {
    //                 Label = labelType,
    //                 Position = ScriptNodePortPosition.Right,
    //                 Type = $"{type}Single",
    //                 Script = $"PUNK.{table}:FromMemory('$memKey')"
    //             }
    //         ]
    //     };
    // }

    // public static ScriptNode Trigger(
    //     ScriptNode node,
    //     string script
    // )
    // {
    //     node.Inputs.Add(new() {
    //         HasMissingScript = false,
    //         MissingScript = "",
    //         Key = "effect",
    //         AllowMultiple = false,
    //         MultipleSeparator = "",
    //         Label = "Effect",
    //         Position = ScriptNodePortPosition.Right,
    //         Postfix = "",
    //         Prefix = "",
    //         Type = "Effect"
    //     });

    //     node.SimpleArgs.Add(new ScriptNodeSimpleArg() {
    //         Key = "text",
    //         Config = new StringArgConfig() {
    //             Default = "",
    //             Multiline = true,
    //             Placeholder = "Enter trigger text",
    //         },
    //         NoScriptIfEmpty = true,
    //         Postfix = "",
    //         Prefix = "",
    //     });

    //     node.SimpleArgs.Add(new ScriptNodeSimpleArg() {
    //         Key = "limit",
    //         Config = new IntegerArgConfig() {
    //             Label = "Triggers per turn (0 if no limit):",
    //             HasMax = false,
    //             HasMin = true,
    //             Max = 0,
    //             Min = 0,
    //             Default = 0,
    //             ZeroMeansEmpty = true,
    //         },
    //         NoScriptIfEmpty = true,
    //         Prefix = "\n:Limit(",
    //         Postfix = ")",
    //     });

    //     node.Outputs.Add(new() {
    //         Label = "Trigger",
    //         Position = ScriptNodePortPosition.Left,
    //         Type = "Trigger",
    //         Script = $"{script}\n:Effects(\n$effect\n)$costs$limit\n:Build()"
    //     });

    //     node.InputArray = new()
    //     {
    //         AddButtonLabel = "Add cost",
    //         Key = "costs",
    //         NoScriptIfEmpty = false,
    //         ScriptPostfix = "",
    //         ScriptPrefix = "",
    //         ItemSeparator = "\n",
    //         ItemPrefix = "\n:Cost(\n",
    //         ItemPostfix = "\n)",
    //         Type = "Cost"
    //     };

    //     return node;
    // }

    // public static ScriptNode Cost(
    //     ScriptNode node,
    //     string script
    // )
    // {
    //     node.Outputs.Add(new()
    //     {
    //         Label = "Cost",
    //         Position = ScriptNodePortPosition.Right,
    //         Type = "Cost",
    //         Script = script
    //     });

    //     return node;
    // }

    // public static ScriptNode Condition(
    //     ScriptNode node,
    //     string script,
    //     string? label = null
    // )
    // {
    //     node.Outputs.Add(new()
    //     {
    //         Script = script,
    //         Label = label ?? "?",
    //         Position = ScriptNodePortPosition.Right,
    //         Type = "Condition"
    //     });

    //     return node;
    // }

    public static ScriptNode Number(
        ScriptNode node,
        string script,
        string? label = null
    )
    {
        node.Outputs.Add(new()
        {
            Script = script,
            Label = label ?? "Number",
            Position = ScriptNodePortPosition.Right,
            Type = "Number"
        });

        return node;
    }

    public static ScriptNode TargetAmount(
        ScriptNode node,
        string script
    )
    {
        node.Outputs.Add(new()
        {
            Script = script,
            Label = "Amount",
            Position = ScriptNodePortPosition.Right,
            Type = "Target.Amount"
        });

        return node;

    }


    // public static void AddTarget(
    //     ScriptNodeCollection scriptNodes,
    //     string type,

    // )

    public static void AddSelect(
        ScriptNodeCollection scriptNodes,
        string type,
        string label,
        string method
    )
    {
        var script = $"""
        Select:{method}()$filters
        """;
        
        var result = new ScriptNode()
        {
            Name = $"Select:{type}",
            Label = label,
            Inputs = [
                // new ScriptNodeInputPort() {
                //     HasMissingScript = true,
                //     MissingScript = "",
                //     Key = "amount",
                //     Label = "Amount",
                //     Position = ScriptNodePortPosition.Left,
                //     AllowMultiple = false,
                //     MultipleSeparator = "",
                //     Postfix = "\n)",
                //     Prefix = "\n:Amount(\n\"$amountSelectTip\",\n",
                //     Type = "Number"
                // }
            ],
            Outputs = [
                new() {
                    Label = "Select",
                    Position = ScriptNodePortPosition.Right,
                    Type = $"{type}Select",
                    Script = script
                },
                new() {
                    Label = "Many",
                    Position = ScriptNodePortPosition.Right,
                    Type = $"{type}Many",
                    Script = $"{script}\n:Many()"
                }
            ],
            InputArray = new()
            {
                Key = "filters",
                Type = $"{type}Filter",
                AddButtonLabel = "Add filter" ,
                ScriptPrefix = "\n",
                ScriptPostfix = "",
                NoScriptIfEmpty = true,
                ItemSeparator = "\n",
                ItemPostfix = "",
                ItemPrefix = "",
                Position = ScriptNodePortPosition.Left,
            },
            SimpleArgs = [
                // new ScriptNodeSimpleArg() {
                //     Config = new BoolArgConfig() {
                //         TrueScript = "\n:Single(\"$selectTip\")",
                //         FalseScript = "",
                //         Label = "Single",
                //     },
                //     Key = "single",
                //     Prefix = "",
                //     Postfix = ""
                // },
                // new ScriptNodeSimpleArg() {
                //     Config = new StringArgConfig() {
                //         Default = "",
                //         Multiline = true,
                //         Placeholder = "Enter select tip",
                //     },
                //     Key = "selectTip",
                //     Postfix = "",
                //     Prefix = "",
                // },
                // new ScriptNodeSimpleArg() {
                //     Config = new StringArgConfig() {
                //         Default = "",
                //         Multiline = false,
                //         Placeholder = "Enter memory key",
                //     },
                //     Key = "memKey",
                //     Prefix = "\n:Remember('",
                //     Postfix = "')",
                //     NoScriptIfEmpty = true,
                // }
            ],
            Description = $"{type} selector"
        };

        scriptNodes.Nodes.Add(result);

        // var only = Filter
        // (
        //     new()
        //     {
        //         Name = $"Only_{type}Filter",
        //         Label = "Only",
        //         Inputs = [
        //             Inputs.Single(
        //                 "item",
        //                 type,
        //                 "Only this"
        //             )
        //         ],
        //         Outputs = [],
        //         InputArray = null,
        //         SimpleArgs = [],
        //         Description = "TODO"
        //     },
        //     "",
        //     type,
        //     ":Only(\n$item\n)",
        //     method
        // );

        // scriptNodes.Nodes.Add(only);

        // var except = Filter
        // (
        //     new()
        //     {
        //         Name = $"Except_{type}Filter",
        //         Label = "Except",
        //         Inputs = [
        //             Inputs.Single(
        //                 "item",
        //                 type,
        //                 "Except this"
        //             )
        //         ],
        //         Outputs = [],
        //         InputArray = null,
        //         SimpleArgs = [],
        //         Description = "TODO"
        //     },
        //     "",
        //     type,
        //     ":Except(\n$item\n)",
        //     method

        // );

        // scriptNodes.Nodes.Add(except);

        // var choose = Effect(
        //     new()
        //     {
        //         Name = $"Choose{type}_Effect",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [
        //             Inputs.Select(
        //                 "select",
        //                 type,
        //                 "Select"
        //             )
        //         ],
        //         Label = "Choose",
        //         Outputs = [],
        //         SimpleArgs = []
        //     },
        //     """
        //     PUNK.Effects:Choose(
        //     $select
        //     )
        //     """
        // );
        
        // scriptNodes.Nodes.Add(choose);

        // var inMemory = Filter(
        //     new()
        //     {
        //         Name = $"InMemory_{type}Filter",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [],
        //         Label = "In memory",
        //         Outputs = [],
        //         SimpleArgs = [
        //             new() {
        //                 Config = new StringArgConfig() {
        //                     Default = "",
        //                     Multiline = false,
        //                     Placeholder = "Enter memory key"
        //                 },
        //                 Key = "memKey",
        //                 Postfix = "",
        //                 Prefix = "",
        //                 NoScriptIfEmpty = false,
        //             },
        //         ],
        //     },
        //     "From memory",
        //     type,
        //     """
        //     :InMemory("$memKey")
        //     """,
        //     method

        // );
        
        // scriptNodes.Nodes.Add(inMemory);
    }

    public static ScriptNode Filter(
        ScriptNode node,
        string label,
        string type,
        string script
        // string selectMethod
    )
    {
        node.Outputs.Add(new()
        {
            Script = script,
            Position = ScriptNodePortPosition.Right,
            Label = $"{label} (filter)",
            Type = $"{type}Filter"
        });

        // node.Outputs.Add(new()
        // {
        //     Script = $"PUNK.Select:{selectMethod}(){script}",
        //    Position = ScriptNodePortPosition.Right,
        //    Label = $"{label} (select)",
        //    Type = $"{type}Select"
        // });

        return node;
    }

    public static ScriptNode OneShot(
        ScriptNode node,
        string script
    )
    {

        node.Inputs.Insert(0, new()
        {
            Key = "nextEffect",
            Prefix = ",\n",
            Postfix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            HasMissingScript = true,
            MissingScript = "",
            Label = "Next effect",
            Position = ScriptNodePortPosition.Right,
            Type = "SingleEffect"
        });
        node.Outputs.Add(new()
        {
            Label = "This",
            Position = ScriptNodePortPosition.Left,
            Type = "SingleEffect",
            Script = $"{script}$nextEffect"
        });
        return node;
    }

    public static ScriptNode Cost(
        ScriptNode node,
        string script
    )
    {
        node.Inputs.Insert(0, new()
        {
            Key = "nextCost",
            Prefix = ",\n",
            Postfix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            HasMissingScript = true,
            MissingScript = "",
            Label = "Next cost",
            Position = ScriptNodePortPosition.Right,
            Type = "Cost",
        });
        node.Outputs.Add(new()
        {
            Label = "This",
            Position = ScriptNodePortPosition.Left,
            Type = "Cost",
            Script = $"{script}$nextCost",
        });

        return node;
    }

    public static ScriptNode Mana(
        string manaType
    )
    {
        return new()
        {
            Name = $"Mana.Fixed:{manaType}",
            Label = $"{manaType} mana",
            Inputs = [
                new()
                {
                    Key = "nextMana",
                    Prefix = ",\n",
                    Postfix = "",
                    AllowMultiple = false,
                    MultipleSeparator = "",
                    HasMissingScript = true,
                    MissingScript = "",
                    Label = "Next mana",
                    Position = ScriptNodePortPosition.Right,
                    Type = "Mana",
                },
            ],
            Outputs = [
                new()
                {
                    Label = "This",
                    Position = ScriptNodePortPosition.Left,
                    Type = "Mana",
                    Script = $"Mana.Fixed:{manaType}($amount)$nextMana",
                },
            ],
            Description = "TODO",
            InputArray = null,
            SimpleArgs = [
                new() {
                    Key = "amount",
                    Config = new IntegerArgConfig() {
                        Label = "Amount: ",
                        HasMax = false,
                        HasMin = true,
                        Max = 0,
                        Min = 0,
                        Default = 0,
                        ZeroMeansEmpty = true,
                    },
                    NoScriptIfEmpty = false,
                    Prefix = "",
                    Postfix = "",
                }
            ],
        };
    }

    // public static ScriptNode Modifier(
    //     ScriptNode node,
    //     string method,
    //     string script
    // )
    // {
    //     node.SimpleArgs.Insert(0, new ScriptNodeSimpleArg() {
    //         Key = "text",
    //         Config = new StringArgConfig() {
    //             Default = "",
    //             Multiline = true,
    //             Placeholder = "Enter modifier text",
    //         },
    //         NoScriptIfEmpty = true,
    //         Postfix = "",
    //         Prefix = "",
    //     });

    //     node.Outputs.Add(new()
    //     {
    //         Label = "Modifier",
    //         Position = ScriptNodePortPosition.Left,
    //         Script = $"PUNK.Build:{method}(\"$text\"){script}$conditions\n:Build()",
    //         Type = "Modifier",
    //     });

    //     node.InputArray = new()
    //     {
    //         AddButtonLabel = "Add condition",
    //         ItemPrefix = "\n:Condition(\n",
    //         ItemPostfix = "\n)",
    //         ItemSeparator = "",
    //         Key = "conditions",
    //         NoScriptIfEmpty = true,
    //         ScriptPostfix = "",
    //         ScriptPrefix = "",
    //         Type = "Condition"
    //     };
    //     return node;
    // }

}

public static class Inputs
{

    public static ScriptNodeInputPort Many(
        string key,
        string type,
        string label,
        string? missingScript = null
    )
    {
        return new() {
            Key = key,
            Prefix = "",
            Postfix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            HasMissingScript = missingScript is not null,
            MissingScript = missingScript ?? "",
            Label = label,
            Position = ScriptNodePortPosition.Left,
            Type = $"{type}Many"
        };
    }

    public static ScriptNodeInputPort Number(
        string key,
        string label,
        string? missingScript = null
    )
    {
        return new() {
            Key = key,
            Prefix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            Postfix = "",
            HasMissingScript = missingScript is not null,
            MissingScript = missingScript ?? "",
            Label = label,
            Position = ScriptNodePortPosition.Left,
            Type = "Number"
        };
    }

    public static ScriptNodeInputPort Mana(
        string key,
        string label
    )
    {
        return new() {
            Key = key,
            Prefix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            Postfix = "",
            HasMissingScript = false,
            MissingScript = "",
            Label = label,
            Position = ScriptNodePortPosition.Right,
            Type = "Mana"
        };
    }

    public static ScriptNodeInputPort ManaGroup(
        string key,
        string label
    )
    {
        return new() {
            Key = key,
            Prefix = "",
            AllowMultiple = false,
            MultipleSeparator = "",
            Postfix = "",
            HasMissingScript = false,
            MissingScript = "",
            Label = label,
            Position = ScriptNodePortPosition.Right,
            Type = "ManaGroup"
        };
    }

    // public static ScriptNodeInputPort Condition(
    //     string key,
    //     string label
    // )
    // {
    //     return new() {
    //         Key = key,
    //         Prefix = "",
    //         AllowMultiple = false,
    //         MultipleSeparator = "",
    //         Postfix = "",
    //         HasMissingScript = false,
    //         MissingScript = "",
    //         Label = label,
    //         Position = ScriptNodePortPosition.Left,
    //         Type = "Condition"
    //     };
    // }
    
    // public static ScriptNodeInputPort Single(
    //     string key,
    //     string type,
    //     string label
    // )
    // {
    //     return new()
    //     {
    //         Type = $"{type}Single",
    //         HasMissingScript = false,
    //         MissingScript = "",
    //         AllowMultiple = false,
    //         MultipleSeparator = "",
    //         Key = key,
    //         Label = label,
    //         Position = ScriptNodePortPosition.Left,
    //         Postfix = "",
    //         Prefix = "",
    //     };
    // }

}

public static class SimpleArgs
{

    // public static ScriptNodeSimpleArg Comparators(
    //     string key
    // )
    // {
    //     return new ScriptNodeSimpleArg() {
    //         Key = key,
    //         Prefix = "PUNK.CmpOps.",
    //         Postfix = "",
    //         Config = new EnumArgConfig()
    //         {
    //             Values = new() {
    //                 { ">", "Gt" },
    //                 { "≥", "Gte" },
    //                 { "<", "Lt" },
    //                 { "≤", "Lte" },
    //                 { "=", "Eq" },
    //                 { "≠", "Neq" },
    //             }
    //         }
    //     };
    // }

}

public static class ArgConfigs
{

    // public static EnumArgConfig Colors()
    // {
    //     return new()
    //     {
    //         Values = new()
    //         {
    //             { "Red", "Red" },
    //             { "Blue", "Blue" },
    //             { "Yellow", "Yellow" },
    //             { "Green", "Green" },
    //         }
    //     };
    // }

    // public static EnumArgConfig Types()
    // {
    //     return new()
    //     {
    //         Values = new()
    //         {
    //             { "Program", "Program" },
    //             { "Gear", "Gear" },
    //             { "Legend", "Legend" },
    //             { "Unit", "Unit" },
    //         }
    //     };
    // }

    // public static EnumArgConfig Areas()
    // {
    //     return new()
    //     {
    //         Values = new()
    //         {
    //             { "Legends", "Legends" },
    //             { "Units", "Units" },
    //             { "Eddies", "Eddies" },
    //         }
    //     };
    // }

    // public static EnumArgConfig Keywords()
    // {
    //     return new()
    //     {
    //         Values = new()
    //         {
    //             { "BLOCKER", "PUNK.Keywords.BLOCKER" },
    //             { "ADRENALINE", "PUNK.Keywords.ADRENALINE" },
    //         }
    //     };
    // }

}