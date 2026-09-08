-- {T}: Add {G}.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {G}.')
                :Cost(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Add {G}.')
                        :CanProduceMana()
                        :Effects(
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