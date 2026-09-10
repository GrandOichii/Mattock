-- {T}: Add {B}.

function _Create()
    return New:Card()
        :ActivatedAbilities(
            New:ActivatedAbility('{T}: Add {B}.')
                :Costs(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Add {B}.')
                        :CanProduceMana()
                        :Effects(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Group(
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