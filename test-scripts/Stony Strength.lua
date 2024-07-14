-- Put a +1/+1 counter on target creature you control. Untap that creature.

function _Create()
    return Card.Spell()
        .Target(
            't1',
            Target.CreatureYouControl
        )
        .Effect(
            Effect.PutCounter(
                Counter.PlusOnePlusOne,
                Target.GetTarget('t1')
            )
        )
        .Effect(
            Effect.Untap(
                Target.GetTarget('t1')
            )
        )
    .Create()
end