using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public record DependencyDescription(
    Type DependencyType,
    DependencyLifetime Lifetime);

