function _Create(children)
    return string.format('.Activated(ActivatedAbility()\n%s\n%s\n.Create())', children[1], children[2])
end