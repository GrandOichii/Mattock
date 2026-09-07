Triggers = {}

function Triggers:_(triggerType) -- TODO? text
    local builder = {
        type = triggerType,
        filters = {},
        
        _zoneSelects = {},
    }

    function builder:_Filter(filter)
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

        builder:_Filter(function (ctx, triggerCtx)
            local card = Card:This()(ctx)
            local zone = GetCardZone(card)
            for _, zoneSelect in ipairs(builder._zoneSelects) do
                if zoneSelect:Match(ctx, zone) then
                    return true
                end
            end
            return false
        end)

        return {
            Type = builder.type,
            Filters = builder.filters,
        }
    end

    return builder
end

function Triggers:OnPermanentEnter()
    error('Triggers:OnPermanentEnter not implemented')
end

function Triggers:StepBeginning()
    local builder = Triggers:_(TriggerTypes.StepBeginning)

    function builder:Steps(...)
        local steps = {...}

        return builder:_Filter(function (ctx, triggerCtx)
            if triggerCtx.StepType == nil then
                return false
            end
            for _, step in ipairs(steps) do
                DEBUG(tostring(step)..'  '..tostring(triggerCtx.StepType))
                if step == triggerCtx.StepType then
                    return true
                end
            end
            return false
        end)
    end

    function builder:PlayerFilter(playersSelect)
        return builder:_Filter(function (ctx, triggerCtx)
            return playersSelect:Match(ctx, triggerCtx.Player)
        end)
    end

    return builder
end