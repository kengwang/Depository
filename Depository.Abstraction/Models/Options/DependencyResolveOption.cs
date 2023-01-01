using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models.Options;

public class DependencyResolveOption
{
    public IDepositoryResolveScope? Scope { get; set; } = null;
}