Permanent = {}

function Permanent:This()
    return function (ctx)
        local card = Card:This()(ctx)
        local result = GetPermanentById(card.Id)
        -- return result
        assert(result ~= nil, 'Permanent:This was called on a card that is not a permanent (card id: '..card.Id..')')
        return result
    end
end