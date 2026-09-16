-- Whenever you gain life, target opponent loses that much life.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever you gain life, target opponent loses that much life.')
                :Triggers(
                    Triggers:LifeGain()
                        :PlayerFilter(
                            Select:Players()
                                :You()
                        )
                        :RememberLifeGained('LIFE_GAINED')
                        :Build()
                )
                :Effects(
                    New:Effects('Target opponent loses that much life.')
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
                            OneShot:LoseLife(
                                Select:Players()
                                    :FromTarget('T1')
                                    :Many(),
                                Number:FromMemory('LIFE_GAINED')
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end