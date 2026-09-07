-- At the beginning of your upkeep, you may gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('At the beginning of your upkeep, you may gain 1 life.')
                :Trigger(
                    Triggers:StepBeginning()
                        :Steps(StepTypes.Upkeep)
                        :PlayerFilter(
                            Select:Players()
                                :You()
                        )
                        :Build()
                )
                :Effect(
                    New:Effect('You gain 1 life.')
                        :Effect(
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