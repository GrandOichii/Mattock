Many = {
    AnyTargets = {}
}

function Many.AnyTargets:FromTarget(tgtKey)
    return function (ctx)
        return GetTargetDeclarationCollectionItems(ctx.Targets, tgtKey)
    end
end