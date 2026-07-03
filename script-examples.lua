local New = {}
local Keywords = {}
local Costs = {}
local Acts = {}
local Many = {}
local Mana = {}
local Number = {}
local Single = {}
local Types = {}

-- Thrill-Kill Assassin
-- Deathtouch
-- Unleash

function _Create()
    return New:Card()
        :AddKeyword(Keywords.Deathtouch)
        :AddKeyword(Keywords.Unleash)
        :Build()
end

-- Rakdos Cluestone
function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {B} or {R}.')
                :ManaAbility()
                :Cost(
                    Costs:TapMe()
                )
                :Act(
                    Acts:AddMana(
                        Many:Players()
                            :EffectOwner(),
                        Mana:Choose(
                            Mana:Black(),
                            Mana:Red()
                        )
                    )
                )
                :Build()
        )
        :ActivatedAbility(
            New:ActivatedAbility('{B}{R}, {T}, Sacrifice Rakdos Clueston: Draw a card.')
                :Cost(
                    Costs:Combine(
                        Costs:PayMana(Mana:Black()),
                        Costs:PayMana(Mana:Red()),
                        Costs:TapMe(),
                        Costs:SacrificeMe()
                    )
                )
                :Act(
                    Acts:Draw(
                        Many:Players()
                            :EffectOwner(),
                        Number:Const(1)
                    )
                )
                :Build()
        )
end

-- Riot Spikes
-- Enchant creature
-- Enchanted creature gets +2/-1.

function _Create()
    return New:Card()
        -- TODO enchant target
        :ContinuousEffect(
            New.Acts:PowerToughnessModification('Enchanted creature gets +2/-1.')
                :Permanents(
                    Many:Permanents()
                        :Only(
                            Single.Permanent:Enchanted()
                        )
                )
                :Power(
                    Number:Cost(2)
                )
                :Toughness(
                    Number:Cost(-1)
                )
                :Build()
        )
        :Build()
end

-- Wrecking Ball
-- Destroy target creature or land.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Act('Destroy target creature or land.')
                :PermanentTarget(
                    'T1',
                    Many:Permanents()
                        :Typed(
                            Types.Creature,
                            Types.Land
                        )
                )
                :Act(
                    Acts:Destroy(
                        Many:Permanents()
                            :Target('T1')
                    )
                )
                :Build()
        )
        :Build()
end

-- Divination
-- Draw 2 cards

function _Create()
    return New:Card()
        :SpellEffect(
            New:Act('Draw 2 cards.')
                :Act(
                    Acts:Draw(
                        Many:Players()
                            :You(),
                        Number:Const(2)
                    )
                )
                :Build()
        )
        :Build()
end