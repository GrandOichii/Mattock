-- Cumulative upkeep {1}
-- At the beginning of your draw step, destroy each creature with mana value equal to the number of age counters on Wave of Terror. They can’t be regenerated.

function _Create()
    return Card.Permanent()
        .Static(
            Static.CumulativeUpkeep(
                Cost.Mana(
                    Mana.Generic(1)
                )
            )
        )
        .At(
            At.TheBeginningOfYourDrawStep,
            Effect.DestroyCBR_Filter(
                Filter.And(
                    Filter.Creatures,
                    Filter.ManaValueEqual(
                        Counter.GetCount('age')
                    )
                )
            )
        )
    .Create()
end