-- Destroy target artifact or enchantment.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effect('Destroy target artifact or enchantment.')
                :Target(
                    Target:Permanent(
                        Select:Permanents()
                            :OfTypes(CardTypes.Artifact, CardTypes.Enchantment)
                            :Many(),
                        Number:Const(1)
                    )
                )
                -- TODO
                :Build()
        )
        :Build()
end