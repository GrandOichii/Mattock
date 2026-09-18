-- Destroy target creature.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Destroy target creature.')
                :Targets(
                    Target:Permanent(
                        'T1',
                        Select:Permanents()
                            :OfTypes(CardTypes.Creature),
                        Target.Amount:Exactly(
                            Number:Const(1)
                        )
                    )
                )
                :Effects(
                    OneShot:Destroy(
                        Select:Permanents()
                            :FromTarget('T1')
                            :Many()
                    )
                )
                :Build()
        )
        :Build()
end