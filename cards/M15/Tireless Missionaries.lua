-- When this creature enters, you gain 3 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('When this creature enters, you gain 3 life.')
                :Trigger(
                    Triggers:ETB()
                        :PermanentFilter(
                            Select:Permanents()
                                :Only(Permanent:This()) -- this creature
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('You gain 3 life.')
                        :Effects(
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