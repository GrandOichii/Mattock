-- Draw 2 cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Draw 2 cards.')
                :Effect(
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