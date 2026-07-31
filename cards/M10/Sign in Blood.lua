-- Target player draws 2 cards and loses 2 life.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Target player draws 2 cards and loses 2 life.')
                :Target(
                    Target:Player(
                        'T1',
                        Select:Players(),
                        Target.Amount:Exactly(
                            Number:Const(1)
                        )
                    )
                )
                :Effect(
                    OneShot:Draw(
                        Select:Players()
                            :FromTarget('T1')
                            :Many(),
                        Number:Const(2)
                    )
                )
                :Effect(
                    OneShot:LoseLife(
                        Select:Players()
                            :FromTarget('T1')
                            :Many(),
                        Number:Const(2)
                    )
                )
                :Build()
        )
        :Build()
end