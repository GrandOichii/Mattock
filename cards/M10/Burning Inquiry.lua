-- Each player draws three cards, then discards three cards at random.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Each player draws three cards, then discards three cards at random.')
                :Effects(
                    OneShot:Draw(
                        Select:Players()
                            :Many(),
                        Number:Const(3)
                    ),
                    OneShot:Discard(
                        Select:Players()
                            :Many(),
                        Number:Const(3),
                        true
                    )
                )
                :Build()
        )
        :Build()
end