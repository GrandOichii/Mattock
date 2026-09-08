-- Whenever a creature card is put into an opponent's graveyard from anywhere, you gain 1 life.

function _Create()
    return New:Card()
        :TriggeredAbility(
            New:TriggeredAbility('Whenever a creature card is put into an opponent\'s graveyard from anywhere, you gain 1 life.')
                :Trigger(
                    Triggers:SingleCardZoneChange()
                        :TargetZoneFilter(
                            Zones:Select()
                                :NotOwnedBy(Player:You())
                        )
                        :CardFilter(
                            Select:Cards()
                                :OfType(CardTypes.Creature)
                        )
                        -- TODO
                        :Build()
                )
                :Effects(
                    New:Effects('You gain 1 life.')
                        :Effects(
                            OneShot:GainLife(
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