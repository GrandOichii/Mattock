-- Whenever another creature enters, untap this creature.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever another creature enters, untap this creature.')
                :Triggers(
                    Triggers:ETB()
                        :PermanentFilter(
                            Select:Permanents()
                                :Exept(Permanent:This()) -- Another
                                :OfTypes(CardTypes.Creature) -- Creature
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('Untap this creature.')
                        :Effects(
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