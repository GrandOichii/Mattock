Number = {}

function Number:Const(const)
    return function (ctx)
        return const
    end
end