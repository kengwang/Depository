using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public record DependencyRelation(
    DependencyRelationType RelationType,
    Type ImplementType,
    object? DefaultImplementation = null,
    string? Name = null,
    bool IsEnabled = true)
{
    public bool IsEnabled { get; set; } = IsEnabled;
}