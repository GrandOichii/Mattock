Target = {}

Target.Amount = {}

function Target.Amount:Const(number)
    -- Target player ...
    -- Destroy six target ...
    return function (ctx)
        return number(ctx)
    end
end

function Target.Amount:UpTo(number)
    -- Up to two target creatures ...
    return function (ctx)
        -- TODO
        error('Target.Amount:UpTo not implemented')
    end
end

function Target.Amount:AnyNumber()
    -- Any number of target ...
    return function (ctx)
        return -1
    end
end


function Target:Player(playersSelect, targetAmount)
    return {
        Get = function (ctx)
            local items = playersSelect(ctx)
            local choices = {}
            local amount = targetAmount(ctx)

            -- TODO
            return choices
        end,
        Check = function (ctx)
            return false
        end
    }
end

function Target:Permanent(permanentsSelect, targetAmount)
    return {
        Get = function ()
            -- TODO
            return {}
        end,
        Check = function (ctx)
            return false
        end
    }
end