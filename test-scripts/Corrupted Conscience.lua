function _Create()
    return Card.Aura()
        .Enchant(
            Enchant.Creature
        )
        .Enchanted()
            .Static(
                Static.ChangeControllerToOwner
            )
            .Static(
                Static.Ability.Infect
            )
    .Create()
end