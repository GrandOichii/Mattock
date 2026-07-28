Select = {}

function Select:_(allGetter)
    local select = {
        filters = {}
    }

    function select:_Filter(f)
        select.filters[#select.filters+1] = f
        return select
    end

    function select:Many()
        return function (ctx)
            local all = allGetter()

            local filterFunc = function (item)
                for _, filter in ipairs(select.filters) do
                    if not filter(ctx, item) then
                        return false
                    end
                end
                return true
            end

            local items = {}
            for _, item in ipairs(all) do
                if filterFunc(item) then
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

    return select
end

function Select:Players()
    local select = Select:_(GetPlayersInAPNAP)

    function select:You()
        return select:Only(Player:You())
    end

    function select:Only(player)
        return select:_Filter(function (ctx, p)
            return p == player(ctx)
        end)
    end

    return select
end

function Select:Permanents()
    local select = Select:_(GetPermanents)

    function select:NotOfType(type)
        return select:_Filter(function (ctx, permanent)
            -- TODO
            return false
        end)
    end

    function select:OfTypes(...)
        local types = {...}

        return select:_Filter(function (ctx, permanent)
            for _, type in ipairs(types) do
                if PermanentHasType(permanent, type) then
                    return true
                end
            end
            return false
        end)
    end

    function select:ControlledBy(player)
        return select:_Filter(function (ctx, permanent)
            return GetPermanentController(permanent) == player(ctx)
        end)
    end

    return select
end