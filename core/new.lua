New = {}

function New:Card()
    local builder = {
        spellEffects = {},
        activatedAbilities = {},
    }

    function builder:Build()
        return {
            SpellEffects = builder.spellEffects,
            ActivatedAbilities = builder.activatedAbilities,
        }
    end

    function builder:SpellEffect(effect)
        builder.spellEffects[#builder.spellEffects+1] = effect
        return builder
    end

    function builder:ActivatedAbility(aa)
        builder.activatedAbilities[#builder.activatedAbilities+1] = aa
        return builder
    end

    return builder
end

function New:Effect(text)
    local builder = {
        effects = {},
        targets = {},
    }

    function builder:Build()
        return {
            Text = text,
            Effects = builder.effects,
            Targets = builder.targets,
        }
    end

    function builder:Effect(e)
        builder.effects[#builder.effects+1] = e
        return builder
    end

    function builder:Target(target)
        builder.targets[#builder.targets+1] = target
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

    function builder:Cost(cost)
        builder.costs[#builder.costs+1] = cost
        return builder
    end

    function builder:ManaCost(...)
        local costs = {...}
        for _, cost in ipairs(costs) do
            builder.manaCosts[#builder.manaCosts+1] = cost
        end
        return builder
    end

    function builder:Effect(effect)
        builder.effects[#builder.effects+1] = effect
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