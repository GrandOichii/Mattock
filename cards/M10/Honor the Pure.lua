-- White creatures you control get +1/+1.

function _Create()
    return New:Card()
        :StaticAbilities(
            New:StaticAbility('White creatures you control get +1/+1.')
                :Zones(
                    Select:Zones()
                        :Battlefield()
                )
                :Continuous(
                    Continuous:PowerToughnessModification()
                        :Permanents(
                            Select:Permanents()
                                :OfColors(Colors.White)
                                :ControlledBy(Player:You())
                        )
                        :PowerAdd(
                            Number:Const(1)
                        )
                        :ToughnessAdd(
                            Number:Const(1)
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end
