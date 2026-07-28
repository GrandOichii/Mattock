Select = {}

function Select:Players()
    local select = {
        filters = {}
    }

    function select:_Filter(f)
        select.filters[#select.filters+1] = f
        return select
    end

    function select:You()
        return select:Only(Player:You())
    end

    function select:Only(player)
        return select:_Filter(function (ctx, p)
            return p == player(ctx)
        end)
    end

    function select:Many()
        return function (ctx)
            local all = GetPlayersInAPNAP()

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

    return select
end

function Select:Permanents()
    local select = {}

    function select:NotOfType(type)
        -- TODO
        return select
    end

    function select:OfTypes(...)
        local types = {...}

        -- TODO
        return select
    end

    function select:Many()
        -- TODO
        return {}
    end

    function select:ControlledBy(player)
        -- TODO
        return {}
    end

    function select:Count()
        -- TODO
        return -1
    end

    return select
end