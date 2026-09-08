-- Destroy all nonland permanents.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Destroy all nonland permanents.')
                :Effects(
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