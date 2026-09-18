Triggers = {}

function Triggers:_(triggerType) -- TODO? text
    local builder = {
        type = triggerType,
        filters = {},
        
        _zoneSelects = {},
    }

    function builder:_AddFilter(filter)
        builder.filters[#builder.filters+1] = filter
        return builder
    end

    function builder:Zone(zonesSelect)
        builder._zoneSelects[#builder._zoneSelects+1] = zonesSelect
        return builder
    end

    function builder:Build()
    -- TODO this feels weird: shouldn't this be in core?
        if #builder._zoneSelects == 0 then
            builder:Zone(
                Select:Zones()
                    :WithNames(ZoneNames.Battlefield)
            )
        end

        return {
            Type = builder.type,
            Filters = builder.filters,
        }
    end

    builder:_AddFilter(function (ctx, triggerCtx)
        local card = Card:This()(ctx)
        local zone = GetCardZone(card)
        for _, zoneSelect in ipairs(builder._zoneSelects) do
            if zoneSelect:Match(ctx, zone) then
                return true
            end
        end
        return false
    end)

    return builder
end

function Triggers:ETB()
    local builder = Triggers:_(TriggerTypes.ETB)

    function builder:PermanentFilter(permanentsSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return permanentsSelect:Match(ctx, triggerCtx.Permanent)
        end)
    end

    return builder
end

function Triggers:SpellCast()
    local builder = Triggers:_(TriggerTypes.SpellCast)

    function builder:CasterFilter(playersSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Caster)
        end)
    end

    function builder:CardFilter(cardsSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return cardsSelect:Match(ctx, triggerCtx.Card)
        end)
    end

    return builder
end

function Triggers:LifeGain()
    local builder = Triggers:_(TriggerTypes.LifeGain)

    function builder:PlayerFilter(playersSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Player)
        end)
    end

    function builder:RememberLifeGained(memKey)
        return builder:_AddFilter(function (ctx, triggerCtx)
            ctx.Memory[memKey] = triggerCtx.Gained

            return true
        end)
    end

    return builder
end

function Triggers:SingleDiscard()
    local builder = Triggers:_(TriggerTypes.SingleDiscard)

    function builder:PlayerFilter(playersSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Player)
        end)
    end

    function builder:RememberPlayer(memKey)
        return builder:_AddFilter(function (ctx, triggerCtx)
            ctx.Memory[memKey] = triggerCtx.Player
            return true
        end)
    end

    return builder
end

function Triggers:SingleDraw()
    local builder = Triggers:_(TriggerTypes.SingleDraw)

    function builder:PlayerFilter(playersSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Player)
        end)
    end

    function builder:RememberPlayer(memKey)
        return builder:_AddFilter(function (ctx, triggerCtx)
            ctx.Memory[memKey] = triggerCtx.Player
            return true
        end)
    end

    return builder
end

function Triggers:SingleDeath()
    local builder = Triggers:_(TriggerTypes.SingleDeath)

    function builder:PermanentFilter(permanentsSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return permanentsSelect:Match(ctx, triggerCtx.Permanent)
        end)
    end

    builder:PermanentFilter(
        Select:Permanents()
            :OfTypes(CardTypes.Creature)
    )

    return builder
end

function Triggers:StepBeginning()
    local builder = Triggers:_(TriggerTypes.StepBeginning)

    function builder:Steps(...)
        local steps = {...}

        return builder:_AddFilter(function (ctx, triggerCtx)
            if triggerCtx.StepType == nil then
                return false
            end
            for _, step in ipairs(steps) do
                if step == triggerCtx.StepType then
                    return true
                end
            end
            return false
        end)
    end

    function builder:PlayerFilter(playersSelect)
        return builder:_AddFilter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Player)
        end)
    end

    return builder
end
