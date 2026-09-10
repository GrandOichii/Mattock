-- Target player discards 2 cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Target player discards 2 cards.')
                :Targets(
                    Target:Player(
                        Select:Players(),
                        Number:Const(1)
                    )
                )
                :Effects(
                    OneShot:Discard(
                        Select:Players()
                            :FromTarget('T1')
                            :Many(),
                        Number:Const(2),
                        false
                    )
                )
                :Build()
        )
        :Build()
end