-- {B}{B}, {T}: Destroy target green creature.

function _Create()
    return Card.Permanent()
        .Activated(
            ActivatedAbility()
                .Cost(
                    Cost.Mana(
                        Mana.Black,
                        Mana.Black
                    )
                )
                .Cost(
                    Cost.Tap
                )
                .Target(
                    't1',
                    Target.GreenCreature
                )
                .Effect(
                    Effect.Destroy(
                        Target.GetTarget(
                            't1'
                        )
                    )
                )
        )
    .Create()
end