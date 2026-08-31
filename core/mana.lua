Mana = {
    Fixed = {},
}

function Mana:_(text, manaType, number)
    return {
        Text = text,
        Type = manaType,
        GetAmount = number,
    }
end

function Mana:Generic(text, number)
    return Mana:_(text, -1, number)
end

function Mana:White(text, number)
    return Mana:_(text, ManaTypes.White, number)
end

function Mana:Blue(text, number)
    return Mana:_(text, ManaTypes.Blue, number)
end

function Mana:Black(text, number)
    return Mana:_(text, ManaTypes.Black, number)
end

function Mana:Red(text, number)
    return Mana:_(text, ManaTypes.Red, number)
end

function Mana:Green(text, number)
    return Mana:_(text, ManaTypes.Green, number)
end

function Mana:Colorless(text, number)
    return Mana:_(text, ManaTypes.Colorless, number)
end

function Mana.Fixed:_(manaType, int)
    local text = ''
    local s = '{'

    if      manaType == ManaTypes.White then s = s..'W'
    elseif  manaType == ManaTypes.Blue then s = s..'U'
    elseif  manaType == ManaTypes.Black then s = s..'B'
    elseif  manaType == ManaTypes.Red then s = s..'R'
    elseif  manaType == ManaTypes.Green then s = s..'G'
    elseif  manaType == ManaTypes.Colorless then s = s..'C'
    end

    s = s..'}'
    for _ = 1, int do
        text = text..s
    end

    return Mana:_(
        text,
        manaType,
        Number:Const(int)
    )
end


function Mana.Fixed:Generic(int)
    return Mana.Fixed:_(-1, int)
end

function Mana.Fixed:White(int)
    return Mana.Fixed:_(ManaTypes.White, int)
end

function Mana.Fixed:Blue(int)
    return Mana.Fixed:_(ManaTypes.Blue, int)
end

function Mana.Fixed:Black(int)
    return Mana.Fixed:_(ManaTypes.Black, int)
end

function Mana.Fixed:Red(int)
    return Mana.Fixed:_(ManaTypes.Red, int)
end

function Mana.Fixed:Green(int)
    return Mana.Fixed:_(ManaTypes.Green, int)
end

function Mana.Fixed:Colorless(int)
    return Mana.Fixed:_(ManaTypes.Colorless, int)
end

function Mana:Group(...)
    local mana = {...}

    return function (ctx)
        return mana
    end
end

function Mana:Choose(...)
    local groups = {...}

    return function (ctx)
        local mana = {}
        local texts = {}
        for _, g in ipairs(groups) do
            mana[#mana+1] = g(ctx)

            if Util:GetIdx(texts, mana.Text) > 0 then
                error('Mana:Choose detected two mana costs that share the same text')
            end

            texts[#texts+1] = mana.Text
        end

        error('Mana:Choose not implemented')
        -- TODO call ChooseString to pick the mana text
        -- TODO based on returned text get the idx using Util:GetIdx
        -- TODO return the indexed mana
    end
end