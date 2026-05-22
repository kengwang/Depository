using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    public IDepositoryResolveScope RootScope { get; }
    private IDepositoryResolveScope? CurrentScope { get; set; }
    public IDepositoryResolveScope CreateScope(DepositoryResolveScopeOption? option = null)
    {
        return DepositoryResolveScope.Create(option);
    }

    public IDepository CreateDepositoryInScope(IDepositoryResolveScope scope)
    {
        return new Depository(this, scope);
    }

    public Depository(Depository depository, IDepositoryResolveScope scope)
    {
        _dependencyDescriptions = depository._dependencyDescriptions;
        _dependencyDescriptionsByType = depository._dependencyDescriptionsByType;
        _dependencyRelations = depository._dependencyRelations;
        _currentFocusing = depository._currentFocusing;
        _activationMetadataCache = depository._activationMetadataCache;
        _closedGenericImplementations = depository._closedGenericImplementations;
        RootScope = depository.RootScope;
        Option = depository.Option;
        CurrentScope = scope;
    }
}
