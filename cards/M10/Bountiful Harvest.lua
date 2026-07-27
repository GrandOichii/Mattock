-- You gain 1 life for each land you control.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('You gain 1 life for each land you control.')
                :Effect(
                    OneShot:GainLife(
                        Select:Players()
                            :You()
                            :Many(),
                        Select:Permanents()
                            :OfTypes(CardTypes.Land)
                            :ControlledBy(Player:You())
                            :Count()
                    )
                )
                :Build()
        )
        :Build()
end