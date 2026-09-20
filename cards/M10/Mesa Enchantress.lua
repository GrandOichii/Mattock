-- Whenever you cast an enchantment spell, you may draw a card.

function _Create()
    return New:Card()
        :TriggeredAbilities(
            New:TriggeredAbility('Whenever you cast an enchantment spell, you may draw a card.')
                :Triggers(
                    Triggers:SpellCast()
                        :CasterFilter(
                            Select:Players()
                                :You()
                        )
                        :CardFilter(
                            Select:Cards()
                                :OfTypes(CardTypes.Enchantment)
                        )
                        :Build()
                )
                :Effects(
                    New:Effects('You may draw a card.')
                        :Effects(
                            OneShot:May(
                                'Draw a card?',
                                OneShot:Draw(
                                    Select:Players()
                                        :You()
                                        :Many(),
                                    Number:Const(1)
                                )
                            )
                        )
                        :Build()
                )
                :Build()
        )
        :Build()
end
