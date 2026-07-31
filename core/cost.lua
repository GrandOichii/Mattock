Cost = {}

function Cost:_(text, payFunc, checkFunc)
    return {
        Pay = payFunc,
        Check = checkFunc,
        Text = text,
    }
end

function Cost:SelfTap()
    return Cost:_(
        '{T}',
        function ()
            -- TODO
        end,
        function ()
            -- TODO
            return false
        end
    )
end