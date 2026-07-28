Player = {}

function Player:You()
    return function (ctx)
        return ctx.Data.Owner
    end
end