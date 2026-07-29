Target = {}

Target.Amount = {}

function Target.Amount:Const(number)
    -- Target player discards 2 cards.
    -- TODO
end

function Target.Amount:UpTo(number)
    -- Up to two target creatures ...
    -- TODO
end

function Target.Amount:AnyNumber()
    -- Any number of target 
    -- TODO
end


function Target:Player(playersSelect, targetAmount)
    return {
        Get = function ()
            -- TODO
            return {}
        end,
        EnoughTargets = function ()
            return false
        end
    }
end