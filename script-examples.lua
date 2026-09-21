-- static abilities

ContinuousEffectLayers = {}
Continuous = {}

----==== Layer 1: Rules and effects that modify copiable values are applied ====---- 

-- Clone
-- You may have this creature enter as a copy of any creature on the battlefield.

function _Create()
    return New:Card()
        -- TODO
        :Build()
end

----==== Layer 2: Control-changing effects are applied ====---- 

-- Mind Control
-- Enchant creature
-- You control enchanted creature.

function _Create()
    return New:Card()
        -- TODO enchant
        :StaticAbilities(
            New:StaticAbility('You control enchanted creature.')
                :Zones(
                    Select:Zones()
                        :Battlefield()
                )
                :Continuous(
                    Continuous:ControlChanging()
                        :Permanents(
                            Select:Permanents()
                                :AuraHost()
                        )
                        :NewController(
                            Player:You()
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end

----==== Layer 3: Text-changing effects are applied ====---- 

-- TODO

----==== Layer 4: Type-changing effects are applied ====---- 

-- TODO

----==== Layer 5: Color-changing effects are applied ====---- 

-- Transguild Courier
-- Transguild Courier is all colors.

function _Create()
    return New:Card()
        :StaticAbilities(
            New:StaticAbility('Transguild Courier is all colors.')
                :CharacteristicDefining()
                :Continuous(
                    Continuous:ColorChanging()
                        :Cards(
                            Select:Cards()
                                :This()
                        )
                        :AddColors(
                            Colors.White,
                            Colors.Blue,
                            Colors.Black,
                            Colors.Red,
                            Colors.Green
                        )
                        :Build()
                )
        )
        :Build()
end

----==== Layer 6: Ability-adding effects, keyword counters, ability-removing effects, and effects that say an object can't have an ability are applied ====---- 

-- Stormfront Pegasus
-- Flying

function _Create()
    return New:Card()
        :StaticAbilities(
            New:StaticAbility('Flying')
                :Continuous(
                    Continuous.Keywords:Flying()
                )
                :Build()
        )
        :Build()
end

----==== Layer 7: Power- and/or toughness-changing effects are applied ====---- 

---=== Layer 7a: Effects from characteristic-defining abilities that define power and/or toughness are applied ===--- 

-- Nightmare
-- Flying
-- Nightmare's power and toughness are each equal to the number of Swamps you control.

function _Create()
    return New:Card()
        :StaticAbilities(
            New:StaticAbility('Flying')
                :Continuous(
                    Continuous.Keywords:Flying()
                )
                :Build(),
            New:StaticAbility('Nightmare\'s power and toughness are each equal to the number of Swamps you control.')
                :CharacteristicDefining()
                :Continuous(
                    Continuous:PowerToughnessDefinition()
                        :Cards(
                            Select:Cards()
                                :This()
                        )
                        :Power(
                            Select:Permanents()
                                :OfSubtypes('Swamp')
                                :ControlledBy(Player:You())
                                :Count()
                        )
                        :Toughness(
                            Select:Permanents()
                                :OfSubtypes('Swamp')
                                :ControlledBy(Player:You())
                                :Count()
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end

---=== Layer 7b: Effects that set power and/or toughness to a specific number or value are applied. ===--- 

-- Utter Insignificance
-- Flash
-- Enchant creature
-- Enchanted creature loses all abilities and has base power and toughness 1/1.
-- {2}{C}: Exile enchanted creature.
function _Create()
    return New:Card()
        -- TODO
        :StaticAbilities(
            New:StaticAbility('Enchanted creature loses all abilities and hase base power and toughness 1/1.')
                -- TODO first part
                :Zones(
                    Select:Zones()
                        :Battlefield()
                )
                :Continuous(
                    Continuous:PowerToughnessSetting()
                        :Permanents(
                            Select:Permanents()
                                :AuraHost()
                        )
                        :Power(
                            Number:Const(1)
                        )
                        :Toughness(
                            Number:Const(1)
                        )
                        :Build()
                )
                :Build()
        )
        -- TODO
        :Build()
end

---=== Layer 7c: Effects and counters that modify power and/or toughness are applied. ===--- 

-- Honor of the Pure
-- White creatures you control get +1/+1.

function _Create()
    return New:Card()
        :StaticAbilities(
            New:StaticAbility('White creatures you control get +1/+1.')
                :Zones(
                    Select:Zones()
                        :Battlefield()
                )
                :Continuous(
                    Continuous:PowerToughnessModification()
                        :Permanents(
                            Select:Permanents()
                                :OfColors(Colors.White)
                                :ControlledBy(Player:You())
                        )
                        :PowerAdd(
                            Number:Const(1)
                        )
                        :ToughnessAdd(
                            Number:Const(1)
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end

---=== Layer 7d: Effects that switch a creature's power and toughness are applied. ===--- 

-- TODO
