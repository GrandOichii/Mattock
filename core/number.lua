Number = {}

function Number:Const(int)
    return function (ctx)
        return int
    end
end

function Number:FromMemory(memKey)
    return function (ctx)
        return ctx.Memory[memKey]
    end
end