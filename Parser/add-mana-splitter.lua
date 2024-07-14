function _Create(children)
    if #children == 1 then
        return children[1]
    end
    local result = ''
    for i, child in ipairs(children) do
        if child ~= '' then
            if i ~= 1 then
                result = result..',\n'
            end
            result = result..child
        end
    end
    return 'Mana.Or(\n'..result..'\n)'
end