Util = {}

function Util:GetIdx(t, v)
    for i, el in ipairs(t) do
        if el == v then
            return i
        end
    end
    return -1
end