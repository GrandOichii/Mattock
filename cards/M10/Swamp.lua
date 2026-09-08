-- {T}: Add {B}.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {B}.')
                :Cost(
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