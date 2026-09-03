-- When this creature enters, you gain 3 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('When this creature enters, you gain 3 life.')
                :Trigger(
                    Triggers:OnPermanentEnter(
                        Select:Permanents()
                            :Only(Permanent:This())
                    )
                )
                :Effect(
                    New:Effect('You gain 3 life.')
                        :Effect(
                            OneShot:GainLife(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Number:Const(3)
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end