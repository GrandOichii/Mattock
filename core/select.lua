Select = {}

function Select:Players()
    local select = {}

    function select:You()
        -- TODO
        return select
    end

    function select:Many()
        -- TODO
        return {}
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