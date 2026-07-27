New = {}

function New:Card()
    local builder = {
        spellEffects = {},
    }

    function builder:Build()
        return {
            SpellEffects = builder.spellEffects,
        }
    end

    function builder:SpellEffect(effect)
        builder.spellEffects[#builder.spellEffects+1] = effect
        return builder
    end

    return builder
end

function New:Effect(text)
    local builder = {
        effects = {}
    }

    function builder:Build()
        return {
            Text = text,
            Effects = builder.effects,
        }
    end

    function builder:Effect(e)
        builder.effects[#builder.effects+1] = e
        return builder
    end

    return builder
end