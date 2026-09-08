-- Draw 2 cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Draw 2 cards.')
                :Effects(
                    OneShot:Draw(
                        Select:Players()
                            :You()
                            :Many(),
                        Number:Const(2)
                    )
                )
                :Build()
        )
        :Build()
end