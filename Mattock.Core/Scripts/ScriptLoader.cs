using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScriptEditor.Core;
using ScriptEditor.Core.SimpleArgs;

namespace Mattock.Core.Scripts;

public class ScriptLoader
{
    public static ScriptNodeCollection ScriptNodes { get; }

    static ScriptLoader()
    {
        ScriptNodes = new ScriptNodeCollection()
        {
            Nodes = []
        };

        // Unique

        ScriptNodes.Nodes.Add(
            new()
            {
                Name = "New:Card",
                Label = "Card",
                Inputs = [
                    new() {
                        Key = "spellEffect",
                        Prefix = "\n:SpellEffect(\n",
                        Postfix = "\n)",
                        HasMissingScript = true,
                        MissingScript = "",
                        AllowMultiple = false,
                        Label = "Spell effect",
                        Position = ScriptNodePortPosition.Right,
                        Type = "Effect",
                        MultipleSeparator = "",
                    },
                    new() {
                        Key = "activatedAbilities",
                        Prefix = "\n:ActivatedAbilities(\n",
                        Postfix = "\n)",
                        HasMissingScript = true,
                        MissingScript = "",
                        AllowMultiple = false,
                        Label = "Activated abilities",
                        Position = ScriptNodePortPosition.Right,
                        Type = "ActivatedAbilityCollection",
                        MultipleSeparator = "",
                    },
                ],
                Outputs = [
                    new() {
                        Label = "",
                        Position = ScriptNodePortPosition.Left,
                        Type = "Card",
                        Script = """
                        function _Create()
                        return New:Card()$spellEffect$activatedAbilities
                        :Build()
                        end
                        """
                    }
                ],
                Description = "TODO",
                InputArray = null,
                SimpleArgs = [
                    
                ]
            }
        );

        ScriptNodes.Nodes.Add(
            new()
            {
                Name = "New:Effect",
                Label = "Effect",
                Inputs = [
                    new() {
                        Key = "singleEffects",
                        Prefix = "\n:Effects(\n",
                        Postfix = "\n)",
                        HasMissingScript = true,
                        MissingScript = "",
                        AllowMultiple = false,
                        Label = "Single effects",
                        Position = ScriptNodePortPosition.Right,
                        Type = "SingleEffect",
                        MultipleSeparator = "",                        
                    }
                ],
                Outputs = [
                    new() {
                        Label = "",
                        Position = ScriptNodePortPosition.Left,
                        Type = "Effect",
                        Script = """
                        New:Effects('$text')$canProduceMana$targets$singleEffects
                        :Build()
                        """
                    }
                ],
                Description = "TODO",
                InputArray = new()
                {
                    Key = "targets",
                    Type = "Target",
                    AddButtonLabel = "Add target",
                    ScriptPrefix = "\n:Targets(\n",
                    ScriptPostfix = ")",
                    NoScriptIfEmpty = true,
                    ItemSeparator = ",\n",
                    ItemPrefix = "",
                    ItemPostfix = "\n",
                    Position = ScriptNodePortPosition.Left,
                },
                SimpleArgs = [
                    new ScriptNodeSimpleArg() {
                        Config = new StringArgConfig() {
                            Default = "",
                            Multiline = true,
                            Placeholder = "Effect text",
                        },
                        Key = "text",
                        Postfix = "",
                        Prefix = "",
                        NoScriptIfEmpty = false
                    },
                    new ScriptNodeSimpleArg() {
                        Config = new BoolArgConfig() {
                            FalseScript = "",
                            TrueScript = "\n:CanProduceMana()",
                            Label = "Produces mana?",
                        },
                        Key = "canProduceMana",
                        Postfix = "",
                        Prefix = "",
                        NoScriptIfEmpty = false
                    }
                ]
            }
        );

        ScriptNodes.Nodes.Add(
            new()
            {
                Name = "New:ActivatedAbility",
                Label = "Activated ability",
                Inputs = [
                    new() {
                        AllowMultiple = false,
                        HasMissingScript = true,
                        Key = "manaCosts",
                        Label = "Mana costs",
                        MissingScript = "",
                        MultipleSeparator = "",
                        Position = ScriptNodePortPosition.Right,
                        Postfix = "\n)",
                        Prefix = "\n:ManaCosts(\n",
                        Type = "Mana",
                    },
                    new() {
                        AllowMultiple = false,
                        HasMissingScript = true,
                        Key = "costs",
                        Label = "Costs",
                        MissingScript = "",
                        MultipleSeparator = "",
                        Position = ScriptNodePortPosition.Right,
                        Postfix = "\n)",
                        Prefix = "\n:Costs(\n",
                        Type = "Cost",
                    },
                    // TODO duplicated code
                    new() {
                        Key = "singleEffects",
                        Prefix = "\n:Effects(\n",
                        Postfix = "\n)",
                        HasMissingScript = true,
                        MissingScript = "",
                        AllowMultiple = false,
                        Label = "Single effects",
                        Position = ScriptNodePortPosition.Right,
                        Type = "Effect",
                        MultipleSeparator = "",                        
                    }
                ],
                Outputs = [
                    new() {
                        Label = "",
                        Position = ScriptNodePortPosition.Left,
                        Type = "ActivatedAbility",
                        Script = """
                        New:ActivatedAbility('$text')$manaCosts$costs$singleEffects
                        :Build()
                        """,
                    }
                ],
                Description = "TODO",
                InputArray = null,
                SimpleArgs = [
                    new ScriptNodeSimpleArg() {
                        Config = new StringArgConfig() {
                            Default = "",
                            Multiline = true,
                            Placeholder = "Activated ability text",
                        },
                        Key = "text",
                        Postfix = "",
                        Prefix = "",
                        NoScriptIfEmpty = false
                    }
                ]
            }
        );

        ScriptNodes.Nodes.Add(
            new()
            {
                Name = "ActivatedAbilities",
                Label = "Activated ability collection",
                Inputs = [],
                Outputs = [
                    new() {
                        Label = "Abilities",
                        Position = ScriptNodePortPosition.Left,
                        Script = "$abilities",
                        Type = "ActivatedAbilityCollection",
                    }
                ],
                Description = "TODO",
                InputArray = new()
                {
                    AddButtonLabel = "Add ability",
                    ItemPostfix = "",
                    ItemPrefix = "",
                    ItemSeparator = ",\n",
                    Key = "abilities",
                    NoScriptIfEmpty = false,
                    Position = ScriptNodePortPosition.Right,
                    ScriptPostfix = "",
                    ScriptPrefix = "",
                    Type = "ActivatedAbility"
                },
                SimpleArgs = [],
            }
        );

        // Single effects

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:Draw",
                    Label = "Draw cards",
                    Inputs = [
                        Inputs.Many(
                            "players",
                            "Player",
                            "Players"
                        ),
                        Inputs.Number(
                            "amount",
                            "Amount"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:Draw(
                $players,
                $amount
                )
                """
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:TapPermanents",
                    Label = "Tap permanents",
                    Inputs = [
                        Inputs.Many(
                            "permanents",
                            "Permanent",
                            "Permanents"
                        ),
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:TapPermanents(
                $permanents
                )
                """
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:UntapPermanents",
                    Label = "Untap permanents",
                    Inputs = [
                        Inputs.Many(
                            "permanents",
                            "Permanent",
                            "Permanents"
                        ),
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:UntapPermanents(
                $permanents
                )
                """
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:AddMana",
                    Label = "Add mana",
                    Inputs = [
                        Inputs.Many(
                            "players",
                            "Player",
                            "Players"
                        ),
                        Inputs.ManaGroup(
                            "mana",
                            "Mana group"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:AddMana(
                $players,
                $mana
                )
                """
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:GainLife",
                    Label = "Gain life",
                    Inputs = [
                        Inputs.Many(
                            "players",
                            "Player",
                            "Players"
                        ),
                        Inputs.Number(
                            "amount",
                            "Amount"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:GainLife(
                $players,
                $amount
                )
                """
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:LoseLife",
                    Label = "Lose life",
                    Inputs = [
                        Inputs.Many(
                            "players",
                            "Player",
                            "Players"
                        ),
                        Inputs.Number(
                            "amount",
                            "Amount"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:LoseLife(
                $players,
                $amount
                )
                """
            )
        );
        
        ScriptNodes.Nodes.Add(
            Scripts.OneShot
            (
                new()
                {
                    Name = "OneShot:Mill",
                    Label = "Mill",
                    Inputs = [
                        Inputs.Many(
                            "players",
                            "Player",
                            "Players"
                        ),
                        Inputs.Number(
                            "amount",
                            "Amount"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                """
                OneShot:Mill(
                $players,
                $amount
                )
                """
            )
        );

        // Costs
        ScriptNodes.Nodes.Add(
            Scripts.Cost(
                new()
                {
                    Name = "Cost:SelfTap",
                    Label = "Tap self",
                    Inputs = [],
                    Outputs = [],
                    Description = "TODO",
                    InputArray = null,
                    SimpleArgs = []
                },
                """
                Cost:SelfTap()
                """
            )
        );

        // Mana

        string[] manaTypes = [
            "Generic",
            "White",
            "Blue",
            "Black",
            "Red",
            "Green",
        ];

        foreach (var manaType in manaTypes)
        {
            ScriptNodes.Nodes.Add(
                Scripts.Mana(manaType)
            );
        }

        // Mana groups

        ScriptNodes.Nodes.Add(new()
        {
            Name = "Mana:Group",
            Label = "Mana group",
            Inputs = [
                Inputs.Mana(
                    "mana",
                    "Mana"
                ),
            ],
            Outputs = [
                new() {
                    Label = "",
                    Position = ScriptNodePortPosition.Left,
                    Type = "ManaGroup",
                    Script = """
                    Mana:Group(
                    $mana
                    )
                    """
                }
            ],
            Description = "TODO",
            InputArray = null,
            SimpleArgs = []
        });

        // Selects

        Scripts.AddSelect
        (
            ScriptNodes,
            "Player",
            "Players",
            "Players"
        );

        Scripts.AddSelect
        (
            ScriptNodes,
            "Permanent",
            "Permanents",
            "Permanents"
        );
        
        // Targets

        ScriptNodes.Nodes.Add(
            Scripts.Target(
                "Player"
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.Target(
                "Permanent"
            )
        );
        
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "Target:Player",
        //         Label = "Target player",
        //         Inputs = [
        //             new() {
        //                 Key = "players",
        //                 Prefix = "",
        //                 Postfix = "",
        //                 HasMissingScript = false,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",                        
        //                 Label = "Player select",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "PlayerSelect",
        //             },
        //             new() {
        //                 Key = "amount",
        //                 Prefix = "",
        //                 Postfix = "",
        //                 HasMissingScript = false,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Amount",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "Target.Amount",
        //             },
        //         ],
        //         Outputs = [
        //             new() {
        //                 Label = "Target",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Target",
        //                 Script = """
        //                 Target:Player(
        //                 '$tgtKey',
        //                 $players,
        //                 $amount
        //                 )
        //                 """,
        //             },
        //             new() {
        //                 Label = "Many",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "PlayerMany",
        //                 Script = """
        //                 Select:Players()
        //                 :FromTarget('$tgtKey')
        //                 :Many()
        //                 """,
        //             },
        //         ],
        //         Description = "TODO",
        //         InputArray = null,
        //         SimpleArgs = [
        //             new ScriptNodeSimpleArg() {
        //                 Config = new StringArgConfig() {
        //                     Default = "",
        //                     Multiline = false,
        //                     Placeholder = "Target key",
        //                 },
        //                 Key = "tgtKey",
        //                 Postfix = "",
        //                 Prefix = "",
        //                 NoScriptIfEmpty = false
        //             }
        //         ],
        //     }
        // );

        // Target amounts
        ScriptNodes.Nodes.Add(
            Scripts.TargetAmount
            (
                new()
                {
                    Name = "Target.Amount:Exactly",
                    Label = "Exactly",
                    Inputs = [
                        Inputs.Number(
                            "number",
                            "Number"
                        )
                    ],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "Exact number of targets"
                },
                """
                Target.Amount:Exactly(
                $number
                )
                """
            )
        );

        // Filters

        // Player filters
        ScriptNodes.Nodes.Add(
            Scripts.Filter
            (
                new()
                {
                    Name = "Player:Select.You",
                    Label = "You",
                    Inputs = [],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                "You",
                "Player",
                ":You()"
                // "Players"
            )
        );

        ScriptNodes.Nodes.Add(
            Scripts.Filter
            (
                new()
                {
                    Name = "Player:Select.Opponents",
                    Label = "Opponents",
                    Inputs = [],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [],
                    Description = "TODO"
                },
                "Opponents",
                "Player",
                ":Opponents()"
                // "Players"
            )
        );

        // Permanent filters
        ScriptNodes.Nodes.Add(
            Scripts.Filter
            (
                new()
                {
                    Name = "Permanents:Select.OfTypes",
                    Label = "Of types",
                    Inputs = [],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [
                        new ScriptNodeArrayArg() {
                            Key = "types",
                            NoScriptIfEmpty = false,
                            Postfix = "",
                            Prefix = "",
                            AddButtonText = "Add type",
                            ItemPostfix = "'",
                            ItemPrefix = "'",
                            Separator = ", ",
                            Config = ArgConfigs.Colors(),
                        },
                    ],
                    Description = "TODO",
                },
                "",
                "Permanent",
                ":OfTypes($types)"
            )
        );

        // Numbers

        ScriptNodes.Nodes.Add(
            Scripts.Number
            (
                new()
                {
                    Name = "Number:Const",
                    Label = "Const",
                    Inputs = [],
                    Outputs = [],
                    InputArray = null,
                    SimpleArgs = [
                        new ScriptNodeSimpleArg() {
                            Config = new IntegerArgConfig() {
                                HasMin = false,
                                Min = 0,
                                HasMax = false,
                                Max = 0,
                            },
                            Key = "value",
                            Prefix = "",
                            Postfix = ""
                        }
                    ],
                    Description = "A constant number"
                },
                "Number:Const($value)"
            )
        );

        // // Unique
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "Card",
        //         Label = "Card",
        //         Inputs = [
        //             new() {
        //                 Key = "program",
        //                 Prefix = "\n:ProgramEffect(\n",
        //                 Postfix = "\n)",
        //                 HasMissingScript = true,
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 MissingScript = "",
        //                 Label = "Program effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Ability"
        //             },
        //             new() {
        //                 Key = "playEffect",
        //                 Prefix = "\n:Play(\n",
        //                 Postfix = "\n)",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Play effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Ability"
        //             },
        //             new() {
        //                 Key = "call",
        //                 Prefix = "\n:Call(\n",
        //                 Postfix = "\n)",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Call effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Ability"
        //             },
        //             new() {
        //                 Key = "attack",
        //                 Prefix = "\n:Attack(\n",
        //                 Postfix = "\n)",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Attack effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Ability"
        //             },
        //             new() {
        //                 Key = "defeated",
        //                 Prefix = "\n:Defeated(\n",
        //                 Postfix = "\n)",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Defeated effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Ability"
        //             },
        //             new() {
        //                 Key = "handMods",
        //                 Prefix = "\n:HandModifier(\n",
        //                 Postfix = "\n)",
        //                 AllowMultiple = true,
        //                 MultipleSeparator = "",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 Label = "Hand modifiers",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Modifier",
        //             },
        //             new() {
        //                 Key = "ipMods",
        //                 Prefix = "\n:InPlayModifier(\n",
        //                 Postfix = "\n)",
        //                 AllowMultiple = true,
        //                 MultipleSeparator = "",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 Label = "In-play modifiers",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Modifier",
        //             },
        //             new() {
        //                 Key = "activatedAbilities",
        //                 Prefix = "\n:ActivatedAbility(\n",
        //                 Postfix = "\n)",
        //                 AllowMultiple = true,
        //                 MultipleSeparator = "",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 Label = "Activated abilities",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "ActivatedAbility",
        //             },
        //             new() {
        //                 Key = "triggers",
        //                 Prefix = "\n:Trigger(\n",
        //                 Postfix = "\n)",
        //                 AllowMultiple = true,
        //                 MultipleSeparator = "",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 Label = "Triggers",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Trigger",
        //             },
        //             new() {
        //                 Key = "playDuringAttackCosts",
        //                 Prefix = "\n:AllowPlayDuringAttack(\n\"$playDuringAttackText\",\n",
        //                 Postfix = "\n)",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 HasMissingScript = true,
        //                 MissingScript = "",
        //                 Label = "Costs for playing during attack",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "Cost",
        //             },
        //         ],
        //         Outputs = [
        //             new() {
        //                 Label = "",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "Card",
        //                 Script = """
        //                 function _Create()
        //                 return PUNK.Build:Card()$playEffect$attack$call$defeated$program$handMods$ipMods$canAttack$hasBlocker$hasAdrenaline$goSolo$activatedAbilities$triggers$playDuringAttackCosts$spentUnitHaste$rivalHaste
        //                 :Build()
        //                 end
        //                 """
        //             }
        //         ],
        //         Description = "TODO",
        //         InputArray = null,
        //         SimpleArgs = [
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "\n:CantAttack()",
        //                     TrueScript = "",
        //                     Default = true,
        //                     Label = "Can attack",
        //                 },
        //                 Key = "canAttack",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "",
        //                     TrueScript = "\n:Blocker()",
        //                     Default = false,
        //                     Label = "Blocker",
        //                 },
        //                 Key = "hasBlocker",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "",
        //                     TrueScript = "\n:Adrenaline()",
        //                     Default = false,
        //                     Label = "Adrenaline",
        //                 },
        //                 Key = "hasAdrenaline",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "",
        //                     TrueScript = "\n:GoSolo()",
        //                     Default = false,
        //                     Label = "Go Solo",
        //                 },
        //                 Key = "goSolo",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "",
        //                     TrueScript = "\n:CanAttackSpentUnitsThisTurn()",
        //                     Default = false,
        //                     Label = "Spent unit haste",
        //                 },
        //                 Key = "spentUnitHaste",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new BoolArgConfig() {
        //                     FalseScript = "",
        //                     TrueScript = "\n:CanAttackRivalThisTurn()",
        //                     Default = false,
        //                     Label = "Rival haste",
        //                 },
        //                 Key = "rivalHaste",
        //                 Postfix = "",
        //                 Prefix = "",
        //             },
        //             new ScriptNodeSimpleArg() {
        //                 Config = new StringArgConfig() {
        //                     Default = "",
        //                     Multiline = true,
        //                     Placeholder = "Enter play during attack costs text",
        //                 },
        //                 Key = "playDuringAttackText",
        //                 Postfix = "",
        //                 Prefix = "",
        //                 NoScriptIfEmpty = false
        //             }
        //         ]
        //     }
        // );

        

        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "Ability",
        //         Label = "Ability",
        //         Inputs = [
        //             new() {
        //                 Key = "effect",
        //                 Prefix = "",
        //                 Postfix = "",
        //                 HasMissingScript = false,
        //                 MissingScript = "",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "Effect"
        //             }
        //         ],
        //         Outputs = [
        //             new() {
        //                 Label = "Ability",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "Ability",
        //                 Script = """
        //                 "$text",
        //                 $effect
        //                 """
        //             }
        //         ],
        //         InputArray = null,
        //         SimpleArgs = [
        //             new ScriptNodeSimpleArg() {
        //                 Key = "text",
        //                 Config = new StringArgConfig() {
        //                     Multiline = true,
        //                     Default = "",
        //                     Placeholder = "Enter ability text",
        //                 },
        //                 Postfix = "",
        //                 Prefix = ""
        //             }
        //         ],
        //         Description = "TODO"
        //     }
        // );
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "ActivatedAbility",
        //         Label = "Activated ability",
        //         Description = "TODO",
        //         Inputs = [
        //             new() {
        //                 HasMissingScript = false,
        //                 MissingScript = "",
        //                 Key = "effect",
        //                 AllowMultiple = false,
        //                 MultipleSeparator = "",
        //                 Label = "Effect",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Postfix = "",
        //                 Prefix = "",
        //                 Type = "Effect"
        //             }
        //         ],
        //         SimpleArgs = [
        //             new ScriptNodeSimpleArg() {
        //                 Key = "text",
        //                 Config = new StringArgConfig() {
        //                     Default = "",
        //                     Multiline = true,
        //                     Placeholder = "Enter ability text",
        //                 },
        //                 NoScriptIfEmpty = true,
        //                 Postfix = "",
        //                 Prefix = "",
        //             }
        //         ],
        //         InputArray = new()
        //         {
        //             AddButtonLabel = "Add cost",
        //             Key = "costs",
        //             NoScriptIfEmpty = false,
        //             ScriptPostfix = "",
        //             ScriptPrefix = "",
        //             ItemSeparator = "\n",
        //             ItemPrefix = "\n:Cost(\n",
        //             ItemPostfix = "\n)",
        //             Type = "Cost"
        //         },
        //         Outputs = [
        //             new() {
        //                 Label = "Ability",
        //                 Position = ScriptNodePortPosition.Left,
        //                 Type = "ActivatedAbility",
        //                 Script = """
        //                 PUNK.Build:ActivatedAbility("$text")$costs
        //                 :Effects(
        //                 $effect
        //                 )
        //                 :Build()
        //                 """
        //             }
        //         ]
        //     }
        // );

        // // Triggers
        // ScriptNodes.Nodes.Add(
        //     Scripts.Trigger(
        //         new()
        //         {
        //             Name = "IPCSpentTrigger_Trigger",
        //             Label = "In-play card spent trigger",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcFilter",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 ),
        //             ],
        //             SimpleArgs = [
                        
        //             ],
        //             Outputs = [
                        
        //             ]
        //         },
        //         """
        //         PUNK.Build:IPCSpentTrigger("$text")
        //         :IPCFilter(
        //         $ipcFilter
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Trigger(
        //         new()
        //         {
        //             Name = "UnitAttackTrigger_Trigger",
        //             Label = "Unit attack trigger",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "attackerFilter",
        //                     "InPlayCard",
        //                     "Attacker"
        //                 ),
        //             ],
        //             SimpleArgs = [
        //             ],
        //             Outputs = [
        //             ]
        //         },
        //         """
        //         PUNK.Build:UnitAttackTrigger("$text")
        //         :UnitFilter(
        //         $attackerFilter
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Trigger(
        //         new()
        //         {
        //             Name = "CardPlayTrigger_Trigger",
        //             Label = "Card play trigger",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "cardFilter",
        //                     "MatchCard",
        //                     "Card filter"
        //                 ),
        //                 Inputs.Select(
        //                     "playerFilter",
        //                     "Player",
        //                     "Player filter"
        //                 ),
                        
        //             ],
        //             SimpleArgs = [
        //             ],
        //             Outputs = [
        //             ]
        //         },
        //         """
        //         PUNK.Build:CardPlayTrigger("$text")
        //         :CardFilter(
        //         $cardFilter
        //         )
        //         :PlayerFilter(
        //         $playerFilter
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Trigger(
        //         new()
        //         {
        //             Name = "GigStealTrigger_Trigger",
        //             Label = "Gig theft trigger",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "thiefFilter",
        //                     "InPlayCard",
        //                     "Thief"
        //                 ),
        //             ],
        //             SimpleArgs = [
        //             ],
        //             Outputs = [
        //             ]
        //         },
        //         """
        //         PUNK.Build:GigStealTrigger("$text")
        //         :UnitThiefFilter(
        //         $thiefFilter
        //         )
        //         """
        //     )
        // );

        // // Costs
        // ScriptNodes.Nodes.Add(
        //     Scripts.Cost(
        //         new()
        //         {
        //             Name = "SpendThis_Cost",
        //             Label = "Spend this",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [],
        //             Outputs = [
        //             ],
        //             SimpleArgs = []
        //         },
        //         "PUNK.Costs:SpendThis()"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Cost(
        //         new()
        //         {
        //             Name = "Defeat_Cost",
        //             Label = "Defeat",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "cards",
        //                     "InPlayCard",
        //                     "Cards"
        //                 )
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = []
        //         },
        //         """
        //         PUNK.Costs:Defeat(
        //         $cards
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Cost(
        //         new()
        //         {
        //             Name = "RemoveFromGame_Cost",
        //             Label = "Remove from game",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "cards",
        //                     "InPlayCard",
        //                     "Cards"
        //                 )
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = []
        //         },
        //         """
        //         PUNK.Costs:RemoveFromGame(
        //         $cards
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Cost(
        //         new()
        //         {
        //             Name = "Discard_Cost",
        //             Label = "Discard",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "cards",
        //                     "MatchCard",
        //                     "Cards"
        //                 )
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = []
        //         },
        //         """
        //         PUNK.Costs:Discard(
        //         $cards
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Cost(
        //         new()
        //         {
        //             Name = "Spend_Cost",
        //             Label = "Spend",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [],
        //         },
        //         """
        //         PUNK.Costs:Spend(
        //         $ipcs
        //         )
        //         """
        //     )
        // );

        // // Modifiers
        // ScriptNodes.Nodes.Add(
        //     Scripts.Modifier(
        //         new()
        //         {
        //             Name = "CostMod_Modifier",
        //             Label = "Cost modifier",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "selectors",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Label = "Cards",
        //                     Position = ScriptNodePortPosition.Left,
        //                     Prefix = "\n:Selector(\n",
        //                     Postfix = "\n)",
        //                     Type = "MatchCardSelect",
        //                 },
        //                 new() {
        //                     HasMissingScript = true,
        //                     MissingScript = "",
        //                     AllowMultiple = false,
        //                     MultipleSeparator = "",
        //                     Key = "min",
        //                     Label = "Min",
        //                     Type = "Number",
        //                     Prefix = "\n:Min(\n",
        //                     Postfix = "\n)",
        //                     Position = ScriptNodePortPosition.Left,
        //                 },
        //                 new() {
        //                     HasMissingScript = true,
        //                     MissingScript = "",
        //                     AllowMultiple = false,
        //                     MultipleSeparator = "",
        //                     Key = "max",
        //                     Label = "Max",
        //                     Type = "Number",
        //                     Prefix = "\n:Max(\n",
        //                     Postfix = "\n)",
        //                     Position = ScriptNodePortPosition.Left,
        //                 },
        //                 new() {
        //                     HasMissingScript = true,
        //                     MissingScript = "",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Key = "reduce",
        //                     Label = "Reduce",
        //                     Type = "Number",
        //                     Prefix = "\n:Reduce(\n",
        //                     Postfix = "\n)",
        //                     Position = ScriptNodePortPosition.Left,
        //                 },
        //                 new() {
        //                     HasMissingScript = true,
        //                     MissingScript = "",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Key = "increase",
        //                     Label = "Increase",
        //                     Type = "Number",
        //                     Prefix = "\n:Increase(\n",
        //                     Postfix = "\n)",
        //                     Position = ScriptNodePortPosition.Left,
        //                 },
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = [
                        
        //             ]
        //         },
        //         "CostMod",
        //         """
        //         $min$max$selectors$reduce$increase
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Modifier(
        //         new()
        //         {
        //             Name = "AttackMod_Modifier",
        //             Label = "Attack allowance modifier",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "selectors",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Label = "In-play cards",
        //                     Position = ScriptNodePortPosition.Left,
        //                     Prefix = "\n:Selector(\n",
        //                     Postfix = "\n)",
        //                     Type = "InPlayCardSelect",
        //                 },
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = [
        //                 new ScriptNodeSimpleArg() {
        //                     Key = "cantAttackRival",
        //                     Config = new BoolArgConfig() {
        //                         FalseScript = "\n:CantAttackRival()",
        //                         TrueScript = "",
        //                         Label = "Can attack rival",
        //                         Default = true
        //                     },
        //                     NoScriptIfEmpty = true,
        //                     Postfix = "",
        //                     Prefix = "",
        //                 },
        //                 new ScriptNodeSimpleArg() {
        //                     Key = "cantAttackSpentUnits",
        //                     Config = new BoolArgConfig() {
        //                         FalseScript = "\n:CantAttackSpentUnits()",
        //                         TrueScript = "",
        //                         Label = "Can attack spent units",
        //                         Default = true
        //                     },
        //                     NoScriptIfEmpty = true,
        //                     Postfix = "",
        //                     Prefix = "",
        //                 },
        //             ]
        //         },
        //         "AttackMod",
        //         """
        //         $cantAttackRival$cantAttackSpentUnits$selectors
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Modifier(
        //         new()
        //         {
        //             Name = "PowerMod_Modifier",
        //             Label = "Power modifier",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "selectors",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Label = "In-play cards",
        //                     Position = ScriptNodePortPosition.Left,
        //                     Prefix = "\n:Selector(\n",
        //                     Postfix = "\n)",
        //                     Type = "InPlayCardSelect",
        //                 },
        //                 new() {
        //                     HasMissingScript = true,
        //                     MissingScript = "",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Key = "inc",
        //                     Label = "Increase",
        //                     Type = "Number",
        //                     Prefix = "\n:Increase(\n",
        //                     Postfix = "\n)",
        //                     Position = ScriptNodePortPosition.Left,
        //                 },
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = [
        //             ]
        //         },
        //         "PowerMod",
        //         """
        //         $selectors$inc
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Modifier(
        //         new()
        //         {
        //             Name = "KeywordMod_Modifier",
        //             Label = "Keyword modifier",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "selectors",
        //                     AllowMultiple = true,
        //                     MultipleSeparator = "",
        //                     Label = "In-play cards",
        //                     Position = ScriptNodePortPosition.Left,
        //                     Prefix = "\n:Selector(\n",
        //                     Postfix = "\n)",
        //                     Type = "InPlayCardSelect",
        //                 },
        //             ],
        //             Outputs = [
        //             ],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     AddButtonText = "Add keyword",
        //                     Config = ArgConfigs.Keywords(),
        //                     ItemPrefix = "\n:Keyword(",
        //                     ItemPostfix = ")",
        //                     Key = "keywords",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Separator = "",
        //                     NoScriptIfEmpty = false
        //                 }
        //             ]
        //         },
        //         "KeywordMod",
        //         """
        //         $keywords$selectors
        //         """
        //     )
        // );
        
        // // Conditions
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "Cmp_Condition",
        //             Label = "Compare",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "numeric1",
        //                     "Number 1"
        //                 ),
        //                 Inputs.Number(
        //                     "numeric2",
        //                     "Number 2"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 SimpleArgs.Comparators(
        //                     "op"
        //                 )
        //             ],
        //             Description = "Compares two numeric values"
        //         },
        //         """
        //         PUNK.Conditions:Cmp(
        //         $numeric1,
        //         $op,
        //         $numeric2
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "PaysCost_Condition",
        //             Label = "Pays cost",
        //             Inputs = [
        //                 new() {
        //                     AllowMultiple = false,
        //                     HasMissingScript = false,
        //                     Key = "cost",
        //                     Label = "Cost",
        //                     MissingScript = "",
        //                     MultipleSeparator = "",
        //                     Position = ScriptNodePortPosition.Left,
        //                     Prefix = "",
        //                     Postfix = "",
        //                     Type = "Cost"
        //                 }
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new() {
        //                     Key = "hint",
        //                     Config = new StringArgConfig() {
        //                         Default = "",
        //                         Multiline = false,
        //                         Placeholder = "Enter player prompt"
        //                     },
        //                     Postfix = "",
        //                     Prefix = ""
        //                 },

        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Conditions:PaysCost(
        //         "$hint",
        //         $cost
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "And_Condition",
        //             Label = "And",
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             InputArray = new()
        //             {
        //                 AddButtonLabel = "Add new condition",
        //                 ItemPrefix = "",
        //                 ItemPostfix = "",
        //                 ItemSeparator = ",\n",
        //                 Key = "conditions",
        //                 NoScriptIfEmpty = false,
        //                 ScriptPrefix = "",
        //                 Type = "Condition",
        //                 ScriptPostfix = ""
        //             },
        //             SimpleArgs = [
        //             ],
        //             Description = "Compares two numeric values"
        //         },
        //         """
        //         PUNK.Conditions:And(
        //         $conditions
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "HasGigAtMaxValue_Condition",
        //             Label = "Has gig at max value",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "Checks whether the specified player has a gig at max value"
        //         },
        //         """
        //         PUNK.Conditions:HasGigAtMaxValue(
        //         $player
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "HasPairOfSameSidedDice_Condition",
        //             Label = "Has a pair of same-sided dice",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Conditions:HasPairOfSameSidedDice(
        //         $player
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "TurnOfPlayer_Condition",
        //             Label = "Turn of player",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Conditions:TurnOfPlayer(
        //         $player
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "Accepts_Condition",
        //             Label = "Player accepts prompt",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new() {
        //                     Config = new StringArgConfig() {
        //                         Default = "",
        //                         Multiline = true,
        //                         Placeholder = "Enter player prompt",
        //                     },
        //                     Key = "hint",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     NoScriptIfEmpty = false
        //                 }
        //             ],
        //             Description = "Is true only if the specified player accepts a prompt"
        //         },
        //         """
        //         PUNK.Conditions:Accepts(
        //         "$hint",
        //         $player
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "GigAtMaxValue_Condition",
        //             Label = "Gig is at max value",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "gig",
        //                     "Gig",
        //                     "Gig"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "Checks whether the specified gig is at max value"
        //         },
        //         """
        //         PUNK.Conditions:GigAtMaxValue(
        //         $gig
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Condition
        //     (
        //         new()
        //         {
        //             Name = "PlayerHasGigWithSameValue_Condition",
        //             Label = "Player has gig with same value",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //                 Inputs.Single(
        //                     "gig",
        //                     "Gig",
        //                     "Gig"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Conditions:PlayerHasGigWithSameValue(
        //         $player,
        //         $gig
        //         )
        //         """
        //     )
        // );

        // // Effects
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Crash_Effect",
        //             Label = "Crash",
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "PUNK.Effects:Crash()"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "UntilEndOfTurn_Effect",
        //             Label = "Until end of turn",
        //             Inputs = [
        //                 new() {
        //                     // TODO too many of these, move to Inputs.Modifier
        //                     AllowMultiple = false,
        //                     HasMissingScript = false,
        //                     Key = "modifier",
        //                     Label = "Modifier",
        //                     MissingScript = "",
        //                     MultipleSeparator = "",
        //                     Position = ScriptNodePortPosition.Right,
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Type = "Modifier"
        //                 }
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:UntilEndOfTurn(
        //         $modifier
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "CallLegendForCost_Effect",
        //             Label = "Call legend",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = new()
        //             {
        //                 Key = "costs",
        //                 AddButtonLabel = "Add cost",
        //                 ItemPrefix = ",\n",
        //                 ItemPostfix = "",
        //                 ItemSeparator = "",
        //                 NoScriptIfEmpty = false,
        //                 ScriptPostfix = "",
        //                 ScriptPrefix = "",
        //                 Type = "Cost"
        //             },
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:CallLegendForCost(
        //         $player$costs
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "UntilYourNextTurn_Effect",
        //             Label = "Until your next turn",
        //             Inputs = [
        //                 new() {
        //                     AllowMultiple = false,
        //                     HasMissingScript = false,
        //                     Key = "mod",
        //                     Label = "Modifier",
        //                     MissingScript = "",
        //                     MultipleSeparator = "",
        //                     Position = ScriptNodePortPosition.Right,
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Type = "Modifier"
        //                 }
        //             ],
        //             Outputs = [
        //             ],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:UntilYourNextTurn(
        //         $mod
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Spend_Effect",
        //             Label = "Spend",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "PUNK.Effects:Spend(\n$ipcs\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Defeat_Effect",
        //             Label = "Defeat",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "PUNK.Effects:Defeat(\n$ipcs\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Ready_Effect",
        //             Label = "Ready",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "PUNK.Effects:Ready(\n$ipcs\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "BottomDeck_Effect",
        //             Label = "Bottom-deck",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "PUNK.Effects:BottomDeck(\n$ipcs\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Trash_Effect",
        //             Label = "Trash",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "players",
        //                     "Player",
        //                     "Players"
        //                 ),
        //                 Inputs.Number(
        //                     "amount",
        //                     "Amount"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:Trash(
        //         $players,
        //         $amount
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "ReturnCardsFromTrashToHand_Effect",
        //             Label = "Return cards from trash to hand",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "cards",
        //                     "MatchCard",
        //                     "Cards"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:ReturnCardsFromTrashToHand(
        //         $cards
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "Draw_Effect",
        //             Label = "Draw",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "players",
        //                     "Player",
        //                     "Players"
        //                 ),
        //                 Inputs.Number(
        //                     "amount",
        //                     "Amount"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:Draw(
        //         $players,
        //         $amount
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "ChooseAndDiscard_Effect",
        //             Label = "Choose and discard",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "players",
        //                     "Player",
        //                     "Players"
        //                 ),
        //                 Inputs.Number(
        //                     "amount",
        //                     "Amount"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:ChooseAndDiscard(
        //         $players,
        //         $amount
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "AdjustGigValues_Effect",
        //             Label = "Adjust gig values",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "changer",
        //                     "Player",
        //                     "Changer"
        //                 ),
        //                 Inputs.Select(
        //                     "gigs",
        //                     "Gig",
        //                     "Gigs"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new ScriptNodeSimpleArg() {
        //                     Config = new IntegerArgConfig() {
        //                         Label = "From: ",
        //                         HasMax = false,
        //                         HasMin = false,
        //                         Max = 0,
        //                         Min = 0
        //                     },
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Key = "from",
        //                 },
        //                 new ScriptNodeSimpleArg() {
        //                     Config = new IntegerArgConfig() {
        //                         Label = "To: ",
        //                         HasMax = false,
        //                         HasMin = false,
        //                         Max = 0,
        //                         Min = 0
        //                     },
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Key = "to",
        //                 },
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:AdjustGigValues(
        //         $changer,
        //         $gigs,
        //         $from, $to
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "ModGigValues_Effect",
        //             Label = "Modify gig values",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "changer",
        //                     "Player",
        //                     "Changer"
        //                 ),
        //                 Inputs.Select(
        //                     "gigs",
        //                     "Gig",
        //                     "Gigs"
        //                 ),
        //                 Inputs.Number(
        //                     "mod",
        //                     "Add"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:ModGigValues(
        //         $changer,
        //         $gigs,
        //         $mod
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect
        //     (
        //         new()
        //         {
        //             Name = "SetGigValues_Effect",
        //             Label = "Set gig values",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "changer",
        //                     "Player",
        //                     "Changer"
        //                 ),
        //                 Inputs.Select(
        //                     "gigs",
        //                     "Gig",
        //                     "Gigs"
        //                 ),
        //                 Inputs.Number(
        //                     "value",
        //                     "Value"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:SetGigValues(
        //         $changer,
        //         $gigs,
        //         $value
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect(
        //         new()
        //         {
        //             Name = "Bounce_Effect",
        //             Label = "Return to owners hands",
        //             Inputs = [
        //                 Inputs.Select(
        //                     "ipcs",
        //                     "InPlayCard",
        //                     "In-play cards"
        //                 ),
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:Bounce(
        //         $ipcs
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect(
        //         new()
        //         {
        //             Name = "If_Effect",
        //             Label = "If",
        //             Inputs = [
        //                 Inputs.Condition(
        //                     "condition",
        //                     "?"
        //                 ),
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "true_effect",
        //                     AllowMultiple = false,
        //                     MultipleSeparator = "",
        //                     Label = "True",
        //                     Position = ScriptNodePortPosition.Right,
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Type = "Effect"
        //                 }
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:If(
        //         $condition,
        //         $true_effect
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect(
        //         new()
        //         {
        //             Name = "AtTheEndOfTheTurn_Effect",
        //             Label = "At the end of the turn",
        //             Inputs = [
        //                 new() {
        //                     HasMissingScript = false,
        //                     MissingScript = "",
        //                     Key = "eotEffect",
        //                     AllowMultiple = false,
        //                     MultipleSeparator = "",
        //                     Label = "Effect",
        //                     Position = ScriptNodePortPosition.Right,
        //                     Postfix = "",
        //                     Prefix = "",
        //                     Type = "Effect"
        //                 }
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new() {
        //                     Config = new StringArgConfig() {
        //                         Default = "",
        //                         Multiline = true,
        //                         Placeholder = "Enter effect text"
        //                     },
        //                     Key = "text",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     NoScriptIfEmpty = false
        //                 }
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:AtTheEndOfTheTurn(
        //         "$text",
        //         $eotEffect
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect(
        //         new()
        //         {
        //             Name = "ExecuteProgramEffects_Effect",
        //             Label = "Execute program effects",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 ),
        //                 Inputs.Select(
        //                     "cards",
        //                     "MatchCard",
        //                     "Cards"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:ExecuteProgramEffects(
        //         $player,
        //         $cards
        //         )
        //         """
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Effect(
        //         new()
        //         {
        //             Name = "SearchTopDeckAndTake_Effect",
        //             Label = "Search the top deck",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "searcher",
        //                     "Player",
        //                     "Searcher"
        //                 ),
        //                 Inputs.Number(
        //                     "searchAmount",
        //                     "Search amount"
        //                 ),
        //                 Inputs.Select(
        //                     "filters",
        //                     "MatchCard",
        //                     "Card filters"
        //                 ),
        //                 Inputs.Number(
        //                     "takeAmount",
        //                     "Take amount"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         """
        //         PUNK.Effects:SearchTopDeckAndTake(
        //         $searcher,
        //         $searchAmount,
        //         $filters,
        //         $takeAmount
        //         )
        //         """
        //     )
        // );
        
        // // Filters

        // // Player filters
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "You_PlayerFilter",
        //             Label = "You",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "Filters eveything but the source of the effect"
        //         },
        //         "You",
        //         "Player",
        //         ":You()",
        //         "Players"
        //     )
        // );

        // // In play card filters
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Units_InPlayCardFilter",
        //             Label = "Units",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Units",
        //         "InPlayCard",
        //         ":Units()",
        //         "InPlayCards"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Attacking_InPlayCardFilter",
        //             Label = "Attacking",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Attacking",
        //         "InPlayCard",
        //         ":Attacking()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Classified_InPlayCardFilter",
        //             Label = "Classified",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "classifications",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new classification",
        //                     NoScriptIfEmpty = false,
        //                     Config = new StringArgConfig() {
        //                         Default = "",
        //                         Multiline = false,
        //                         Placeholder = "Enter classification"
        //                     }
        //                 },
        //             ],
        //         },
        //         "Classified cards",
        //         "InPlayCard",
        //         ":Classified($classifications)",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Equipped_InPlayCardFilter",
        //             Label = "Equipped",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Equipped",
        //         "InPlayCard",
        //         ":HasGear()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "EquippedTo_InPlayCardFilter",
        //             Label = "Equipped to",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "host",
        //                     "InPlayCard",
        //                     "Host"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Gear",
        //         "InPlayCard",
        //         """
        //         :EquippedTo(
        //         $host
        //         )
        //         """,
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "HasNoGear_InPlayCardFilter",
        //             Label = "Unequipped",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "With no gear",
        //         "InPlayCard",
        //         ":HasNoGear()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Gear_InPlayCardFilter",
        //             Label = "Gear",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Gear",
        //         "InPlayCard",
        //         ":Gear()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Rival_InPlayCardFilter",
        //             Label = "Rival",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Rival",
        //         "InPlayCard",
        //         ":Rival()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Friendly_InPlayCardFilter",
        //             Label = "Friendly",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Cards",
        //         "InPlayCard",
        //         ":Your()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "This_InPlayCardFilter",
        //             Label = "This",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "This",
        //         "InPlayCard",
        //         ":This()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "NotThis_InPlayCardFilter",
        //             Label = "Not this",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Not this",
        //         "InPlayCard",
        //         ":NotThis()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Spent_InPlayCardFilter",
        //             Label = "Spent",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Spent",
        //         "InPlayCard",
        //         ":Spent()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "CmpPower_InPlayCardFilter",
        //             Label = "Compare power",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "number",
        //                     "Compare with"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 SimpleArgs.Comparators(
        //                     "op"
        //                 )
        //             ],
        //             Description = "TODO"
        //         },
        //         "",
        //         "InPlayCard",
        //         ":CmpPower(\n$op,\n$number\n)",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "CmpCost_InPlayCardFilter",
        //             Label = "Compare cost",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "number",
        //                     "Compare with"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 SimpleArgs.Comparators(
        //                     "op"
        //                 )
        //             ],
        //             Description = "TODO"
        //         },
        //         "",
        //         "InPlayCard",
        //         ":CmpCost(\n$op,\n$number\n)",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "FaceUp_InPlayCardFilter",
        //             Label = "Face-up",
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new() {
        //                     Config = new BoolArgConfig() {
        //                         Label = "Is face-up",
        //                         FalseScript = "FaceDown",
        //                         TrueScript = "FaceUp",
        //                         Default = true,
        //                     },
        //                     Key = "face",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     NoScriptIfEmpty = false
        //                 }
        //             ],
        //             Description = "TODO"
        //         },
        //         "",
        //         "InPlayCard",
        //         ":$face()",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Types_InPlayCardFilter",
        //             Label = "Typed",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "types",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new type",
        //                     NoScriptIfEmpty = false,
        //                     Config = ArgConfigs.Types()
        //                 },
        //             ],
        //         },
        //         "Typed in-play cards",
        //         "InPlayCard",
        //         ":Types($types)",
        //         "InPlayCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "InAreas_InPlayCardFilter",
        //             Label = "In areas",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "areas",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new area",
        //                     NoScriptIfEmpty = false,
        //                     Config = ArgConfigs.Areas()
        //                 },
        //             ],
        //         },
        //         "In-play cards in areas",
        //         "InPlayCard",
        //         ":InAreas($areas)",
        //         "InPlayCards"

        //     )
        // );

        // // Match card filters
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "InTrashOfPlayer_MatchCardFilter",
        //             Label = "In Trash of player",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [],
        //         },
        //         "In Trash of player",
        //         "MatchCard",
        //         ":InTrashOfPlayer(\n$player\n)",
        //         "MatchCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "InHandOfPlayer_MatchCardFilter",
        //             Label = "In hand of player",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [],
        //         },
        //         "In hand of player",
        //         "MatchCard",
        //         ":InHandOfPlayer(\n$player\n)",
        //         "MatchCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Colored_MatchCardFilter",
        //             Label = "Colored",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "colors",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new color",
        //                     NoScriptIfEmpty = false,
        //                     Config = ArgConfigs.Colors()
        //                 },
        //             ],
        //         },
        //         "Colored cards",
        //         "MatchCard",
        //         ":Colored($colors)",
        //         "MatchCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Types_MatchCardFilter",
        //             Label = "Typed",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "types",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new type",
        //                     NoScriptIfEmpty = false,
        //                     Config = ArgConfigs.Types()
        //                 },
        //             ],
        //         },
        //         "Typed cards",
        //         "MatchCard",
        //         ":Types($types)",
        //         "MatchCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Classified_MatchCardFilter",
        //             Label = "Classified",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //                 new ScriptNodeArrayArg() {
        //                     Key = "classifications",
        //                     ItemPrefix = "'",
        //                     ItemPostfix = "'",
        //                     Separator = ", ",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     AddButtonText = "Add new classification",
        //                     NoScriptIfEmpty = false,
        //                     Config = new StringArgConfig() {
        //                         Default = "",
        //                         Multiline = false,
        //                         Placeholder = "Enter classification"
        //                     }
        //                 },
        //             ],
        //         },
        //         "Classified cards",
        //         "MatchCard",
        //         ":Classified($classifications)",
        //         "MatchCards"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "This_MatchCardFilter",
        //             Label = "This",
        //             Description = "TODO",
        //             InputArray = null,
        //             Inputs = [
        //             ],
        //             Outputs = [],
        //             SimpleArgs = [
        //             ],
        //         },
        //         "This",
        //         "MatchCard",
        //         ":This()",
        //         "MatchCards"

        //     )
        // );

        // // Gig filters
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Rival_GigFilter",
        //             Label = "Rival",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Gigs",
        //         "Gig",
        //         ":Rival()",
        //         "Gigs"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "NonFixer_GigFilter",
        //             Label = "Non-fixer",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Gigs",
        //         "Gig",
        //         ":NonFixer()",
        //         "Gigs"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "CmpValue_GigFilter",
        //             Label = "Compare value",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "number",
        //                     "Compare with"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 SimpleArgs.Comparators(
        //                     "op"
        //                 )
        //             ],
        //             Description = "TODO"
        //         },
        //         "Gigs",
        //         "Gig",
        //         """
        //         :CmpValue(
        //         $op,
        //         $number
        //         )
        //         """,
        //         "Gigs"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "CanHaveValue_GigFilter",
        //             Label = "Can have value",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "value",
        //                     "Value"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "Gigs",
        //         "Gig",
        //         """
        //         :CanHaveValue(
        //         $value
        //         )
        //         """,
        //         "Gigs"

        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Filter
        //     (
        //         new()
        //         {
        //             Name = "Friendly_GigFilter",
        //             Label = "Friendly",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [],
        //             Description = "TODO"
        //         },
        //         "Gigs",
        //         "Gig",
        //         ":Friendly()",
        //         "Gigs"
        //     )
        // );
        
        // // Selects
        
        
        // Scripts.AddSelect
        // (
        //     ScriptNodes,
        //     "InPlayCard",
        //     "In-play cards",
        //     "InPlayCards"
        // );
        
        // Scripts.AddSelect
        // (
        //     ScriptNodes,
        //     "Gig",
        //     "Gigs",
        //     "Gigs"
        // );
        
        // Scripts.AddSelect
        // (
        //     ScriptNodes,
        //     "MatchCard",
        //     "Cards",
        //     "MatchCards"
        // );
        
        // // Numbers
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "Const_Number",
        //             Label = "Const",
        //             Inputs = [],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new ScriptNodeSimpleArg() {
        //                     Config = new IntegerArgConfig() {
        //                         HasMin = false,
        //                         Min = 0,
        //                         HasMax = false,
        //                         Max = 0,
        //                     },
        //                     Key = "value",
        //                     Prefix = "",
        //                     Postfix = ""
        //                 }
        //             ],
        //             Description = "A constant number"
        //         },
        //         "PUNK.Number:Const($value)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "If_Number",
        //             Label = "If",
        //             Inputs = [
        //                 Inputs.Condition(
        //                     "condition",
        //                     "Condition"
        //                 ),
        //                 Inputs.Number(
        //                     "num1",
        //                     "True number"
        //                 ),
        //                 Inputs.Number(
        //                     "num2",
        //                     "False number"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "A number decided on a condition"
        //         },
        //         "PUNK.Number:If(\n$condition,\n$num1,\n$num2)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "StreetCred_Number",
        //             Label = "Street cred",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "PUNK.Number:StreetCred(\n$player\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "GigValue_Number",
        //             Label = "Gig value",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "gig",
        //                     "Gig",
        //                     "Gig"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "PUNK.Number:GigValue(\n$gig\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "ValuePairCount_Number",
        //             Label = "Value pair count",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "PUNK.Number:ValuePairCount(\n$player\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "UniqueGigValues_Number",
        //             Label = "Unique gig values",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "player",
        //                     "Player",
        //                     "Player"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "PUNK.Number:UniqueGigValues(\n$player\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "IPCPower_Number",
        //             Label = "In-play card power",
        //             Inputs = [
        //                 Inputs.Single(
        //                     "ipc",
        //                     "InPlayCard",
        //                     "In-play card"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //             ],
        //             Description = "TODO"
        //         },
        //         "PUNK.Number:IPCPower(\n$ipc\n)"
        //     )
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.Number
        //     (
        //         new()
        //         {
        //             Name = "Multiply_Number",
        //             Label = "Multiply",
        //             Inputs = [
        //                 Inputs.Number(
        //                     "number",
        //                     "Number"
        //                 )
        //             ],
        //             Outputs = [],
        //             InputArray = null,
        //             SimpleArgs = [
        //                 new() {
        //                     Config = new IntegerArgConfig() {
        //                         HasMax = false,
        //                         HasMin = false,
        //                         Max = 0,
        //                         Min = 0,
        //                         Default = 1,
        //                         Label = "Mult",
        //                         ZeroMeansEmpty = false
        //                     },
        //                     Key = "mult",
        //                     Postfix = "",
        //                     Prefix = "",
        //                     NoScriptIfEmpty = false
        //                 },
        //             ],
        //             Description = "TODO"
        //         },
        //         "$number\n:Multiply($mult)"
        //     )
        // );

        // // Singles

        // // Single in-play cards
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "Host_InPlayCardSingle",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [],
        //         Label = "Gear host",
        //         SimpleArgs = [],
        //         Outputs = [
        //             new() {
        //                 Label = "Host",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "InPlayCardSingle",
        //                 Script = "PUNK.InPlayCard:Host()"
        //             }
        //         ]
        //     }
        // );
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "EffectSource_InPlayCardSingle",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [],
        //         Label = "This",
        //         SimpleArgs = [],
        //         Outputs = [
        //             new() {
        //                 Label = "This",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "InPlayCardSingle",
        //                 Script = "PUNK.InPlayCard:EffectSource()"
        //             }
        //         ]
        //     }
        // );
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "MostPowerful_InPlayCardSingle",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [
        //             Inputs.Single(
        //                 "player",
        //                 "Player",
        //                 "Of player"
        //             )
        //         ],
        //         Label = "Most powerful",
        //         SimpleArgs = [],
        //         Outputs = [
        //             new() {
        //                 Label = "In-play card",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "InPlayCardSingle",
        //                 Script = "PUNK.InPlayCard:MostPowerful(\n$player\n)"
        //             }
        //         ]
        //     }
        // );
        // ScriptNodes.Nodes.Add(
        //     Scripts.SingleFromMemory(
        //         "In-play card",
        //         "InPlayCard",
        //         "InPlayCard"
        //     )
        // );

        // // Single players
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "You_PlayerSingle",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [],
        //         Label = "You",
        //         SimpleArgs = [],
        //         Outputs = [
        //             new() {
        //                 Label = "Player",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "PlayerSingle",
        //                 Script = "PUNK.Player:EffectOwner()"
        //             }
        //         ]
        //     }
        // );
        // ScriptNodes.Nodes.Add(
        //     new()
        //     {
        //         Name = "Rival_PlayerSingle",
        //         Description = "TODO",
        //         InputArray = null,
        //         Inputs = [],
        //         Label = "Rival",
        //         SimpleArgs = [],
        //         Outputs = [
        //             new() {
        //                 Label = "Player",
        //                 Position = ScriptNodePortPosition.Right,
        //                 Type = "PlayerSingle",
        //                 Script = "PUNK.Player:Rival()"
        //             }
        //         ]
        //     }
        // );

        // // Single gigs
        // ScriptNodes.Nodes.Add(
        //     Scripts.SingleFromMemory(
        //         "Gig",
        //         "Gig",
        //         "Gig"
        //     )
        // );
    }

    public static string Load(string path)
    {
        if (!File.Exists(path))
            throw new Exception($"Script file doesn't exist at path: {path}");
        var data = File.ReadAllText(path);

        return Deserialize(data).GenerateScript(ScriptNodes);
    }

    public static ScriptEditorState Deserialize(string data)
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ListObjectConverter());
        var state = JsonConvert.DeserializeObject<ScriptEditorState>(data, settings)!; // TODO !

        return state;
    }
}

public class ListObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(object);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        
        switch (token.Type)
        {
            case JTokenType.Array:
                var list = new List<object>();
                foreach (var item in (JArray)token)
                {
                    list.Add(item.ToObject<object>(serializer)!);
                }
                return list;
            
            // case JTokenType.Object:
            //     var dict = new Dictionary<string, object>();
            //     foreach (var prop in (token as JObject)!.Properties())
            //     {
            //         dict[prop.Name] = prop.Value.ToObject<object>(serializer);
            //     }
            //     return dict;
            
            default:
                return token.ToObject<object>()!;
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}