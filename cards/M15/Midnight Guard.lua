-- Whenever another creature enters, untap this creature.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('Whenever another creature enters, untap this creature.')
                :Trigger(
                    Triggers:OnPermanentEnter(
                        Select:Permanents()
                            :Exept(Permanent:This())
                    )
                )
                :Effect(
                    New:Effect('Untap this creature.')
                        :Effect(
                            OneShot:UntapPermanents(
                                Select:Permanents()
                                    :Only(Permanent:This())
                                    :Many()
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end