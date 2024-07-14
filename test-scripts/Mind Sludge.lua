-- Target player discards a card for each Swamp you control.

function _Create()
    return Card.Spell()
        .Target(
            't1',
            Target.Player
        )
        .Effect(
            Effect.DiscardFromHand(
                Target.GetTarget('t1'),
                Count(
                    Filter.LandsYouControl(
                        Basic.Swamp
                    )
                )
            )
        )
        
end