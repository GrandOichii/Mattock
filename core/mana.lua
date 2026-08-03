Mana = {}

function Mana:_(manaType, number)
    return {
        Type = manaType,
        GetAmount = number,
    }
end

function Mana:Generic(number)
    return Mana:_(-1, number)
end

function Mana:White(number)
    return Mana:_(ManaTypes.White, number)
end

function Mana:Blue(number)
    return Mana:_(ManaTypes.Blue, number)
end

function Mana:Black(number)
    return Mana:_(ManaTypes.Black, number)
end

function Mana:Red(number)
    return Mana:_(ManaTypes.Red, number)
end

function Mana:Green(number)
    return Mana:_(ManaTypes.Green, number)
end

function Mana:Colorless(number)
    return Mana:_(ManaTypes.Colorless, number)
end

