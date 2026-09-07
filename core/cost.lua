Cost = {}

function Cost:_(text, payFunc, checkFunc)
    return {
        Text = text,
        Pay = payFunc,
        Check = checkFunc,
    }
end

function Cost:SelfTap()
    return Cost:_(
        '{T}',
        function (ctx)
            local source = Card:This()(ctx)
            local permanent = GetPermanentById(source.Id)
            return TapPermanents({permanent})
        end,
        function (ctx)
            local source = Card:This()(ctx)
            local config = GetConfig()
            local permanent = GetPermanentById(source.Id)
            if permanent == nil then
                return false
            end
            if PermanentHasType(permanent, CardTypes.Creature) and PermanentIsSummoningSick(permanent) and config.SummoningSickness then
                return false
            end
            return not PermanentIsTapped(permanent)
        end
    )
end