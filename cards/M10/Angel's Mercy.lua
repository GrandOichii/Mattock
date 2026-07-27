-- You gain 7 life.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('You gain 7 life.')
                :Effect(
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