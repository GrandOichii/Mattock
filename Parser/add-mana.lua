function _Create(children)
    local result = 'Effect.AddMana(\n'
    for i, child in ipairs(children) do
        result = result..child
        if i ~= #children then
            result = result..',\n'
        end
    end
    result = result..'\n)'
    return result
end