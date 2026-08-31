-- {T}: Add {C}.
-- {T}: Add {G} or {B}. This land deals 1 damage to you.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {C}.')
                :Cost(
                    Cost:SelfTap()
                )
                :Effect(
                    New:Effect('Add {C}.')
                        :CanProduceMana()
                        :Effect(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana.Fixed:Colorless(1)
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {G} or {B}. This land deals 1 damage to you.')
                :Cost(
                    Cost:SelfTap()
                )
                :Effect(
                    New:Effect('Add {G} or {B}. This land deals 1 damage to you.')
                        :Effect(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Group(
                                    Mana.Fixed:Green(1),
                                    Mana.Fixed:Black(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end