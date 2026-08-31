-- {T}: Add {G}.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {G}.')
                :Cost(
                    Cost:SelfTap()
                )
                :Effect(
                    New:Effect('Add {G}.')
                        :CanProduceMana()
                        :Effect(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Group(
                                    Mana.Fixed:Green(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end