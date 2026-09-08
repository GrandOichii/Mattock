-- Whenever you attack with two or more creatures, draw a card.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('Whenever you attack with two or more creatures, draw a card.')
                :Trigger(
                    Triggers:OnAttack()
                        :AttackingPlayerFilter(
                            Select:Players()
                                :You()
                        )
                        :AttackingPermanentsCountCmp(
                            Compare:Gte(),
                            Number:Const(2)
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('Draw a card.')
                        :Effects(
                            OneShot:Draw(
                                Select:Players()
                                    :You()
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