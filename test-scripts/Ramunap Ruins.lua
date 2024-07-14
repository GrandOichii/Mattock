function _Create()
    return Card.Spell()
        .Activated(ActivatedAbility()
            .Cost(
                Cost.Tap
            )
            .Cost(
                Cost.PayLife(1)
            )
            .Effect(
                Effect.AddMana(
                    Mana.Red
                )
            )
        .Create())
    .Create()
end