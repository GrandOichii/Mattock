New = {}

function New:Card()
    local builder = {
        spellEffects = {},
        activatedAbilities = {},
        triggeredAbilities = {},
    }

    function builder:Build()
        return {
            SpellEffects = builder.spellEffects,
            ActivatedAbilities = builder.activatedAbilities,
            TriggeredAbilities = builder.triggeredAbilities,
        }
    end

    function builder:SpellEffect(effect)
        builder.spellEffects[#builder.spellEffects+1] = effect
        return builder
    end

    function builder:ActivatedAbilities(...)
        local abilities = {...}
        for _, ability in ipairs(abilities) do
            builder.activatedAbilities[#builder.activatedAbilities+1] = ability
        end
        return builder
    end

    function builder:TriggeredAbilities(...)
        local abilities = {...}
        for _, ability in ipairs(abilities) do
            builder.triggeredAbilities[#builder.triggeredAbilities+1] = ability
        end
        return builder
    end

    return builder
end

function New:Effects(text)
    local builder = {
        effects = {},
        targets = {},
        canProduceMana = false,
    }

    function builder:Build()
        return {
            Text = text,
            Effects = builder.effects,
            Targets = builder.targets,
            CanProduceMana = builder.canProduceMana,
        }
    end

    function builder:CanProduceMana()
        builder.canProduceMana = true
        return builder
    end

    function builder:Effects(...)
        local effects = {...}
        for _, e in ipairs(effects) do
            builder.effects[#builder.effects+1] = e
        end
        return builder
    end

    function builder:Targets(...)
        local targets = {...}
        for _, target in ipairs(targets) do
            builder.targets[#builder.targets+1] = target
        end
        return builder
    end

    return builder
end

function New:ActivatedAbility(text)
    local builder = {
        costs = {},
        effects = {},
        manaCosts = {},
    }

    function builder:Costs(...)
        local costs = {...}
        for _, cost in ipairs(costs) do
            builder.costs[#builder.costs+1] = cost
        end
        return builder
    end

    function builder:ManaCosts(...)
        local costs = {...}
        for _, cost in ipairs(costs) do
            builder.manaCosts[#builder.manaCosts+1] = cost
        end
        return builder
    end

    function builder:Effects(...)
        local effects = {...}
        for _, effect in ipairs(effects) do
            builder.effects[#builder.effects+1] = effect
        end
        return builder
    end

    function builder:Build()
        return {
            Text = text,
            Costs = builder.costs,
            Effects = builder.effects,
            ManaCosts = builder.manaCosts,
        }
    end

    return builder
end

-- TODO this shares a lot of stuff with New:ActivatedAbility
function New:TriggeredAbility(text)
    local builder = {
        effects = {},
        triggers = {},
    }

    function builder:Effects(...)
        local effects = {...}
        for _, effect in ipairs(effects) do
            builder.effects[#builder.effects+1] = effect
        end
        return builder
    end

    function builder:Triggers(...)
        local triggers = {...}
        for _, trigger in ipairs(triggers) do
            builder.triggers[#builder.triggers+1] = trigger
        end
        return builder
    end

    function builder:Build()
        return {
            Text = text,
            Effects = builder.effects,
            Triggers = builder.triggers,
        }
    end

    return builder
end