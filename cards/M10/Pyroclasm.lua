-- Pyroclasm deals 2 damage to each creature.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Pyroclasm deals 2 damage to each creature.')
                :Effect(
                    OneShot:DealDamageToPermanents(
                        Select:Permanents()
                            :OfTypes(CardTypes.Creature)
                            :Many(),
                        Number:Const(2)
                    )
                )
                :Build()
        )
        :Build()
end