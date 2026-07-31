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
        function (ctx)
            local permanent = GetPermanentById(ctx.Data.Object.Id)
            TapPermanents({permanent})
        end,
        function (ctx)
            local config = GetConfig()
            local permanent = GetPermanentById(ctx.Data.Object.Id)
            if permanent == nil then
                return false
            end
            if PermanentIsSummoningSick(permanent) and config.SummoningSickness then
                return false
            end
            return not PermanentIsTapped(permanent)
        end
    )
end