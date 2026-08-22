OneShot = {}


function OneShot:Draw(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        return DrawCards(players, amount)
    end
end

function OneShot:Discard(manyPlayers, number, random)
    return function (ctx)
        -- TODO use random
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        return DiscardCards(players, amount, random)
    end
end

function OneShot:GainLife(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        return GainLife(players, amount)
    end
end

function OneShot:LoseLife(manyPlayers, number)
    return function (ctx)
        local players = manyPlayers(ctx)
        local amount = number(ctx)

        return LoseLife(players, amount)
    end
end

function OneShot:TapPermanents(manyPermanents)
    return function (ctx)
        local permanents = manyPermanents(ctx)

        return TapPermanents(permanents)
    end
end

function OneShot:Destroy(manyPermanents)
    -- TODO
    return {}
end

function OneShot:DealDamageToPermanents(manyPermanents, number)
    return function (ctx)
        local permanents = manyPermanents(ctx)
        local amount = number(ctx)
        local damage = {}
        for _, p in ipairs(permanents) do
            damage[#damage+1] = {
                Permanent = p,
                Amount = amount
            }
        end

        return DealDamageToPermanents(damage)
    end
end

function OneShot:AddMana(manyPlayers, ...)
    local mana = {...}

    return function (ctx)
        local players = manyPlayers(ctx)
        local newMana = {}
        for _, m in ipairs(mana) do
            assert(m.Type ~= -1, 'Provided generic mana for OneShot:AddMana')

            newMana[#newMana+1] = {
                Type = m.Type,
                Amount = m.GetAmount(ctx)
            }
        end

        return AddMana(players, newMana)
    end
end