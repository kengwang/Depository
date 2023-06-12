using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public record DependencyDescription(
    Type DependencyType,
    DependencyResolvePolicy ResolvePolicy,
    DependencyLifetime Lifetime);

