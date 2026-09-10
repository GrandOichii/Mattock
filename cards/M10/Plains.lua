-- {T}: Add {W}.

function _Create()
    return New:Card()
        :ActivatedAbilities(
            New:ActivatedAbility('{T}: Add {W}.')
                :Costs(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Add {W}.')
                        :CanProduceMana()
                        :Effects(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Group(
                                    Mana.Fixed:White(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end