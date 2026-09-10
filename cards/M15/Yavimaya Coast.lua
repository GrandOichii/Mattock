-- {T}: Add {C}.
-- {T}: Add {G} or {B}. This land deals 1 damage to you.

function _Create()
    return New:Card()
        :ActivatedAbilities(
            New:ActivatedAbility('{T}: Add {C}.')
                :Costs(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Add {C}.')
                        :CanProduceMana()
                        :Effects(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Group(
                                    Mana.Fixed:Colorless(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :ActivatedAbilities(
            New:ActivatedAbility('{T}: Add {G} or {B}. This land deals 1 damage to you.')
                :Costs(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Add {G} or {B}. This land deals 1 damage to you.')
                        :CanProduceMana()
                        :Effects(
                            OneShot:AddMana(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Mana:Choose(
                                    Mana:Group(
                                        Mana.Fixed:Green(1)
                                    ),
                                    Mana:Group(
                                        Mana.Fixed:Blue(1)
                                    )
                                )
                            )
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('This land deals 1 damage to you.')
                        :Effects(
                            OneShot:DealDamageToPlayers(
                                Select:Players()
                                    :You()
                                    :Many(),
                                Number:Const(1)
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end