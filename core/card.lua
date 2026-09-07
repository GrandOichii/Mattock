Card = {}

function Card:This()
    return function (ctx)
        return ctx.Data.Source
    end
end