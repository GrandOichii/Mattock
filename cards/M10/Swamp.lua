-- {T}: Add {B}.

function _Create()
    return New:Card()
        :ActivatedAbility(
            New:ActivatedAbility('{T}: Add {B}.')
                :Cost(
                    Cost:SelfTap()
                )
                :Effect(
                    New:Effect('Add {B}.')
                        :CanProduceMana()
                        :Effect(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Black(
                                    Number:Const(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end