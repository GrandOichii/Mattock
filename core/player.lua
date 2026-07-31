Player = {}

function Player:You()
    return function (ctx)
        return ctx.Controller
    end
end