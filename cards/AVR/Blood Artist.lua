-- Whenever this creature or another creature dies, target player loses 1 life and you gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever this creature or another creature dies, target player loses 1 life and you gain 1 life.')
                :Triggers(
                    Triggers:SingleDeath()
                        :Build()
                )
                :Effects(
                    New:Effects('Target player loses 1 life and you gain 1 life.')
                        :Targets(
                            Target:Player(
                                'T1',
                                Select:Players(),
                                Target.Amount:Exactly(
                                    Number:Const(1)
                                )
                            )
                        )
                        :Effects(
                            OneShot:LoseLife(
                                Select:Players()
                                    :FromTarget('T1')
                                    :Many(),
                                Number:Const(1)
                            ),
                            OneShot:GainLife(
                                Select:Players()
                                    :You()
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