using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public class DependencyDescription
{
    public Type DependencyType { get; set; } = null!;
    public DependencyResolvePolicy ResolvePolicy { get; set; }
    public DependencyLifetime Lifetime { get; set; }
}