-- Target opponent mills seven cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Target opponent mills seven cards.')
                :Targets(
                    Target:Player(
                        'T1',
                        Select:Players()
                            :Opponents()
                        ,
                        Target.Amount:Exactly(
                            Number:Const(1)
                        )
                    )
                )
                :Effects(
                    OneShot:Mill(
                        Select:Players()
                            :FromTarget('T1')
                            :Many(),
                        Number:Const(7)
                    )
                )
                :Build()
        )
        :Build()
end