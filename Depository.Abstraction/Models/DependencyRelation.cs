using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public class DependencyRelation
{
    public DependencyRelationType RelationType { get; set; }
    public Type ImplementType { get; set; } = null!;
    public object? DefaultImplementation { get; set; }
    public string? Name { get; set; }
    public bool IsEnabled { get; set; } = true;
}