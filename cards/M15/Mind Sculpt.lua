-- Target opponent mills seven cards.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Target opponent mills seven cards.')
                :Target(
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
                :Effect(
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