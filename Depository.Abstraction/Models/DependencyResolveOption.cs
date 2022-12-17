using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models;

public class DependencyResolveOption
{
    public IDepositoryResolveScope? Scope { get; set; }
}