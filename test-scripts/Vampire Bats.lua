function _Create()
    return Card.Permanent()
        .Static(Static.Ability.Flying)
        .Activated(
            ActivatedAbility()
                .Cost(
                    Cost.Mana(
                        Mana.Black
                    )
                )
                .Effect(
                    Effect.UntilEndOfTurn(
                        Static.ModPT(
                            1,
                            0
                        )
                    )
                )
                .LimitPerTurn(
                    2
                )
            .Create()
        )
    .Create()
end