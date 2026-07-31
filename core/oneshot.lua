OneShot = {}


function OneShot:Draw(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        DrawCards(players, amount)
    end
end

function OneShot:Discard(manyPlayers, number, random)
    return function (ctx)
        -- TODO use random
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        DiscardCards(players, amount, random)
    end
end

function OneShot:GainLife(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        GainLife(players, amount)
    end
end

function OneShot:LoseLife(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        LoseLife(players, amount)
    end
end

function OneShot:TapPermanents(manyPermanents)
    return function (ctx)
        local permanents = manyPermanents(ctx)

        TapPermanents(permanents)
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