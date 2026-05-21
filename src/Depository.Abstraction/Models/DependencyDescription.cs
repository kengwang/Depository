using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public class DependencyDescription
{
    public DependencyDescription([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                                 DependencyLifetime lifetime)
    {
        DependencyType = dependencyType;
        Lifetime = lifetime;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
    public Type DependencyType { get; init; }
    public DependencyLifetime Lifetime { get; init; }
    public DependencyRelation? DecorationRelation { get; set; }

    public void Deconstruct([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] out Type dependencyType, out DependencyLifetime lifetime, out DependencyRelation? decorationRelation)
    {
        dependencyType = DependencyType;
        lifetime = Lifetime;
        decorationRelation = DecorationRelation;
    }

    public override int GetHashCode()
    {
        return DependencyType.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == this) return true;
        return obj is DependencyDescription description && Equals(description);
    }

    private bool Equals(DependencyDescription other)
    {
        return DependencyType == other.DependencyType && Lifetime == other.Lifetime;
    }
}
