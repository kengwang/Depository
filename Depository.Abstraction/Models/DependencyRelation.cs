using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models;

public record DependencyRelation(
    Type ImplementType,
    object? DefaultImplementation = null,
    string? Name = null,
    bool IsEnabled = true,
    bool IsDecorationRelation = false,
    Func<IDepository, object>? ImplementationFactory = null)
{
    public bool IsEnabled { get; set; } = IsEnabled;
    public Func<IDepository, object>? ImplementationFactory { get; set; } = ImplementationFactory;
}