function _Create()
    return Card.Permanent()
        .Activated(ActivatedAbility()
            .Cost(
                Cost.Tap
            )
            .Effect(
                Effect.AddMana(
                    Mana.Green
                )
            )
        .Create())
    .Create()
end