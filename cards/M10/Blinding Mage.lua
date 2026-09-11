-- {W}, {T}: Tap target creature.

-- TODO add tests

function _Create()
    return New:Card()
        :ActivatedAbilities(
            New:ActivatedAbility('{W}, {T}: Tap target creature.')
                :ManaCosts(
                    Mana.Fixed:White(1)
                )
                :Costs(
                    Cost:SelfTap()
                )
                :Effects(
                    New:Effects('Tap target creature.')
                        :Targets(
                            Target:Permanent(
                                'T1',
                                Select:Permanents()
                                    :OfTypes(CardTypes.Creature),
                                Target.Amount:Exactly(
                                    Number:Const(1)
                                )
                            )
                        )
                        :Effects(
                            OneShot:TapPermanents(
                                Select:Permanents()
                                    :FromTarget('T1')
                                    :Many()
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end