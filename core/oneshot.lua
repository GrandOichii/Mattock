OneShot = {}


function OneShot:Draw(manyPlayers, number)
    return function ()
        local players = manyPlayers()
        local amount = number()

        DrawCards(players, amount)
    end
end

function OneShot:GainLife(manyPlayers, number)
    -- TODO
    return {}
end

function OneShot:Destroy(manyPermanents)
    -- TODO
    return {}
end

function OneShot:DealDamageToPermanents(manyPermanents, number)
    -- TODO
    return {}
end