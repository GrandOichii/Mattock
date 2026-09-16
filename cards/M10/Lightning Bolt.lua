-- Lightning Bolt deals 3 damage to any target.

function _Create()
    return New:Card()
        :SpellEffect(
            New:Effects('Lightning Bolt deals 3 damage to any target.')
                :Targets(
                    Target:AnyTarget(
                        'T1',
                        Select:Permanents(),
                        Select:Players(),
                        Target.Amount:Exactly(
                            Number:Const(1)
                        )
                    )
                )
                :Effects(
                    OneShot:DealDamageToAnyTarget(
                        Many.AnyTargets:FromTarget('T1'),
                        Number:Const(3)
                    )
                )
                :Build()
        )
        :Build()
end