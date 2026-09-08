-- You gain 7 life.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('You gain 7 life.')
                :Effects(
                    OneShot:GainLife(
                        Select:Players()
                            :You()
                            :Many(),
                        Number:Const(7)
                    )
                )
                :Build()
        )
        :Build()
end