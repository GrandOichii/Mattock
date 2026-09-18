-- Whenever this creature dies, target opponent discards a card at random.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever this creature dies, target opponent discards a card at random.')
                :Triggers(
                    Triggers:SingleDeath()
                        :PermanentFilter(
                            Select:Permanents()
                                :Only(Permanent:This())
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('Target opponent discards a card at random.')
                        :Targets(
                            Target:Player(
                                'T1',
                                Select:Players()
                                    :Opponents(),
                                Target.Amount:Exactly(
                                    Number:Const(1)
                                )
                            )
                        )
                        :Effects(
                            OneShot:Discard(
                                Select:Players()
                                    :FromTarget('T1')
                                    :Many(),
                                Number:Const(1),
                                true
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end