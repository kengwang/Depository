using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models.Options;

public class DependencyResolveOption
{
    public IDepositoryResolveScope? Scope { get; set; } = null;
    public bool IncludeDisabled { get; set; } = false;
    public string? RelationName { get; set; } = null;
    public Dictionary<Type,object>? FatherImplementations { get; set; } = null;
}