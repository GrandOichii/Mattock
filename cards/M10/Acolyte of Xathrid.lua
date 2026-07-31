-- You gain 7 life.

-- {1}{B}, {T}: Target player loses 1 life.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{1}{B}, {T}: Target player loses 1 life.')
                :ManaCost(
                    Mana:Generic(
                        Number:Const(1)
                    ),
                    Mana:Black(
                        Number:Const(1)
                    )
                )
                :Cost(
                    Cost:SelfTap()
                )
                :Effect(
                    New:Effect('Target player loses 1 life.')
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
                            OneShot:LoseLife(
                                Select:Players()
                                    :FromTarget('T1')
                                    :Many(),
                                Number:Const(1)
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end