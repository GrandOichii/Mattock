Target = {}

Target.Amount = {}

function Target.Amount:_(minFunc, maxFunc)
    local result = {}

    function result:Min(ctx)
        return minFunc(ctx)
    end

    function result:Max(ctx)
        return maxFunc(ctx)
    end

    return result
end

function Target.Amount:Exactly(number)
    -- Target player ...
    -- Destroy six target ...
    return Target.Amount:_(
        function (ctx)
            return number(ctx)
        end,
        function (ctx)
            return number(ctx)
        end
    )
end

function Target.Amount:UpTo(number)
    -- Up to two target creatures ...
    return Target.Amount:_(
        function (ctx)
            return 0
        end,
        function (ctx)
            return number(ctx)
        end
    )
end

function Target.Amount:AnyNumber()
    -- Any number of target ...
    return Target.Amount:_(
        function (ctx)
            return 0
        end,
        function (ctx)
            return -1
        end
    )
end

function Target:_(tgtKey, itemsSelect, targetAmount, chooserFunc, hint)
    return {
        Get = function (ctx)
            local items = itemsSelect:Many()(ctx)
            local player = Player:You()(ctx)

            return {
                Key = tgtKey,
                Items = chooserFunc(
                    player,
                    items,
                    targetAmount:Min(ctx),
                    targetAmount:Max(ctx),
                    hint
                )
            }
        end,
        Check = function (ctx)
            local min = targetAmount:Min(ctx)

            local count = itemsSelect:Count()(ctx)
            if count < min then
                return false
            end

            return true
        end
    }
end

function Target:Player(tgtKey, playersSelect, targetAmount)
    return Target:_(
        tgtKey,
        playersSelect,
        targetAmount,
        ChoosePlayers,
        'Choose players'
    )
end

function Target:Permanent(tgtKey, permanentsSelect, targetAmount)
    return Target:_(
        tgtKey,
        permanentsSelect,
        targetAmount,
        ChoosePermanents,
        'Choose permanents'
    )
end