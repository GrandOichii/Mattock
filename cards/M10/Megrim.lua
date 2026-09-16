-- Whenever an opponent discards a card, Megrim deals 2 damage to that player.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever an opponent discards a card, Megrim deals 2 damage to that player.')
                :Triggers(
                    Triggers:SingleDiscard()
                        :PlayerFilter(
                            Select:Players()
                                :Opponents()
                        )
                        :RememberPlayer('THAT_PLAYER')
                        :Build()
                )
                :Effects(
                    New:Effects('Target opponent loses that much life.')
                        :Effects(
                            OneShot:DealDamageToPlayers(
                                Select:Players()
                                    :FromMemorySingle('THAT_PLAYER')
                                    :Many(),
                                Number:Const(1)
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end