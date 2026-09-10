-- Whenever you cast a white spell or a Plains you control enters, you gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever you cast a white spell or a Plains you control enters, you gain 1 life.')
                :Triggers(
                    Triggers:SpellCast()
                        :CasterFilter(
                            Select:Players()
                                :You()
                        )
                        :CardFilter(
                            Select:Cards()
                                :OfColors(Colors.White)
                        )
                        :Build(),
                    Triggers:ETB()
                        :PermanentFilter(
                            Select:Permanents()
                                :OfSubtypes('Plains')
                                :ControlledBy(Player:You())
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('You gain 1 life.')
                        :Effects(
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