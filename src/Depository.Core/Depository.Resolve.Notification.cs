using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private void NotifyDependencyChange(DependencyDescription dependencyDescription, int mode = 0)
    {
        if (mode is 0 or 1)
        {
            PostTypeChangeNotification(typeof(IEnumerable<>).MakeGenericType(dependencyDescription.DependencyType));
        }

        if (mode is 0 or 2)
        {
            PostTypeChangeNotification(dependencyDescription.DependencyType);
        }
    }

    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private void PostTypeChangeNotification(Type type)
    {
        var notificationType = typeof(INotifyDependencyChanged<>).MakeGenericType(type);
        var description = GetDependencyDescription(notificationType);
        if (description is null) return;

        foreach (var relation in GetRelations(description))
        {
            var result = ResolveRelation(description, relation, description.DependencyType, ResolveContext.Default);
            notificationType.GetMethod(nameof(INotifyDependencyChanged<object>.OnDependencyChanged))!
                .Invoke(result, new object?[] { null });
        }
    }
}
