using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private DependencyRelation? SelectRelation(DependencyDescription dependencyDescription, ResolveContext context)
    {
        if (!context.SkipDecoration && dependencyDescription.DecorationRelation is not null)
            return dependencyDescription.DecorationRelation;

        return GetRelation(dependencyDescription, context.IncludeDisabled, context.RelationName);
    }

    private bool ShouldResolveRelation(DependencyRelation relation, ResolveContext context)
    {
        if (!context.IncludeDisabled && !relation.IsEnabled) return false;
        return context.RelationName is null || relation.Name == context.RelationName;
    }
}
