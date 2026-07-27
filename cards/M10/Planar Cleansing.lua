-- Destroy all nonland permanents.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Destroy all nonland permanents.')
                :Effect(
                    OneShot:Destroy(
                        Select:Permanents()
                            :NotOfType(CardTypes.Land)
                            :Many()
                    )
                )
                :Build()
        )
        :Build()

end