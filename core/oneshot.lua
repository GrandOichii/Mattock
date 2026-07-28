OneShot = {}


function OneShot:Draw(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        DrawCards(players, amount)
    end
end

function OneShot:GainLife(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        GainLife(players, amount)
    end
end

function OneShot:Destroy(manyPermanents)
    -- TODO
    return {}
end

function OneShot:DealDamageToPermanents(manyPermanents, number)
    -- TODO
    return {}
end