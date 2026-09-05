-- Whenever you cast a white spell or a Plains you control enters, you gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('Whenever you cast a white spell or a Plains you control enters, you gain 1 life..')
                :Trigger(
                    Triggers:OnSpellCast()
                        :CasterFilter(
                            Select:Players()
                                :You()
                        )
                        :CardFilter(
                            Select:Cards()
                                :OfColors(Color.White)
                        )
                        :Build()
                )
                :Trigger(
                    Triggers:OnPermanentEnter()
                        :PermanentFilter(
                            Select:Permanents()
                                :OfSubtypes('Plains')
                                :ControlledBy(Player:You())
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