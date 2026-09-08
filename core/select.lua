Select = {}

function Select:_(allGetter)
    local select = {
        filters = {}
    }

    function select:_AddFilter(f)
        select.filters[#select.filters+1] = f
        return select
    end

    function select:Only(single)
        return select:_AddFilter(function (ctx, item)
            return item == single(ctx)
        end)
    end

    function select:Exept(single)
        return select:_AddFilter(function (ctx, item)
            return item ~= single(ctx)
        end)
    end

    function select:FromTarget(tgtKey)
        return select:_AddFilter(function (ctx, item)
            local targets = GetTargetDeclarationCollectionItems(ctx.Targets, tgtKey)
            for _, t in ipairs(targets) do
                if item == t then
                    return true
                end
            end
            return false
        end)
    end

    function select:Many()
        return function (ctx)
            local all = allGetter()

            local items = {}
            for _, item in ipairs(all) do
                if select:Match(ctx, item) then
                    items[#items+1] = item
                end
            end

            return items
        end
    end

    function select:Count()
        return function (ctx)
            local result = select:Many()(ctx)
            return #result
        end
    end

    function select:Match(ctx, item)
        for _, filter in ipairs(select.filters) do
            if not filter(ctx, item) then
                return false
            end
        end
        return true
    end

    return select
end

function Select:Players()
    local select = Select:_(GetPlayersInAPNAP)

    function select:You()
        return select:Only(Player:You())
    end

    function select:Opponents()
        return select:_AddFilter(function (ctx, p)
            local me = Player:You()(ctx)
            return AreOpponents(me, p)
        end)
    end

    return select
end

function Select:Permanents()
    local select = Select:_(GetPermanents)

    function select:NotOfType(type)
        return select:_AddFilter(function (ctx, permanent)
            -- TODO
            return false
        end)
    end

    function select:OfSubtypes(...)
        local subtypes = {...}

        return select:_AddFilter(function (ctx, permanent)
            for _, subtype in ipairs(subtypes) do
                if PermanentHasSubtype(permanent, subtype) then
                    return true
                end
            end
            return false
        end)
    end

    function select:OfTypes(...)
        local types = {...}

        return select:_AddFilter(function (ctx, permanent)
            for _, type in ipairs(types) do
                if PermanentHasType(permanent, type) then
                    return true
                end
            end
            return false
        end)
    end

    function select:ControlledBy(player)
        return select:_AddFilter(function (ctx, permanent)
            return GetPermanentController(permanent) == player(ctx)
        end)
    end

    return select
end

function Select:Zones()
    local select = Select:_(GetZones)

    function select:OwnedBy(player)
        error('Select:OwnedBy not implemented')
    end

    function select:NotOwnedBy(player)
        error('Select:NotOwnedBy not implemented')
    end

    function select:WithNames(...)
        local names = {...}

        return select:_AddFilter(function (ctx, zone)
            for _, name in ipairs(names) do
                if GetZoneName(zone) == name then
                    return true
                end
            end
            return false
        end)
    end

    return select
end

function Select:Cards()
    local select = Select:_(GetCards)

    function select:OfColors(...)
        local colors = {...}

        return select:_AddFilter(function (ctx, card)
            for _, color in ipairs(colors) do
                if CardHasColor(card, color) then
                    return true
                end
            end
            return false
        end)
    end

    return select
end