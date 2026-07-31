-- Destroy target artifact or enchantment.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Destroy target artifact or enchantment.')
                :Target(
                    Target:Permanent(
                        'T1',
                        Select:Permanents()
                            :OfTypes(CardTypes.Artifact, CardTypes.Enchantment),
                        Target.Amount:Exactly(
                            Number:Const(1)
                        )
                    )
                )
                -- TODO
                :Build()
        )
        :Build()
end