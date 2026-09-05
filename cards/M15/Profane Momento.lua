-- Whenever a creature card is put into an opponent's graveyard from anywhere, you gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('Whenever a creature card is put into an opponent\'s graveyard from anywhere, you gain 1 life.')
                :Trigger(
                    Triggers:SingleCardZoneChange()
                        -- TODO
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