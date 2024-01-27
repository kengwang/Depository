using Depository.Abstraction.Models.Options;

namespace Depository.Abstraction.Interfaces;

public interface IScopeDepository
{
    public IDepositoryResolveScope RootScope { get; }
    public IDepositoryResolveScope CreateScope(DepositoryResolveScopeOption? option = null);
    public IDepository CreateDepositoryInScope(IDepositoryResolveScope scope);
}