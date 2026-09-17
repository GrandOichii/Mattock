-- Whenever an opponent draws a card, this enchantment deals 1 damage to that player.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever an opponent draws a card, this enchantment deals 1 damage to that player.')
                :Triggers(
                    Triggers:SingleDraw()
                        :PlayerFilter(
                            Select:Players()
                                :Opponents()
                        )
                        :RememberPlayer('THAT_PLAYER')
                        :Build()
                )
                :Effects(
                    New:Effects('This enchantment deals 1 damage to that player.')
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