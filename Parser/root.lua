function _Create(children)
    -- TODO change card func
    local script = 'function _Create()\nreturn Card.Card()\n%s.Create()\nend'
    return string.format(script, children[1])
end