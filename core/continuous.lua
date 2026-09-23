Continuous = {}

function Continuous:_(layer, text)
    local builder = {
        -- TODO
    }

    function builder:Build()
        return {
            -- TODO
        }
    end

    return builder
end

function Continuous:PowerToughnessModification(text)
    local builder = Continuous:_(
        ContinuousEffectLayers.PowerToughnessModification,
        text
    )

    function builder:Permanents(permanentsSelect)
        error('Continuous:PowerToughnessModification:Permanents not implemented')
    end

    function builder:PowerAdd(number)
        error('Continuous:PowerToughnessModification:PowerAdd not implemented')
    end

    function builder:ToughnessAdd(number)
        error('Continuous:PowerToughnessModification:ToughnessAdd not implemented')
    end

    return builder
end