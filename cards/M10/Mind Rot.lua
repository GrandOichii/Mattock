-- Target player discards 2 cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Target player discards 2 cards.')
                :Target(
                    Target:Player(
                        Select:Players(),
                        Number:Const(1)
                    )
                )
                :Effect(
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