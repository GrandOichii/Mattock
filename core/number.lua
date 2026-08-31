Number = {}

function Number:Const(int)
    return function (ctx)
        return int
    end
end