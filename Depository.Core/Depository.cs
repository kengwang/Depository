using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository : IDepository
{
    private readonly DepositoryOption _option = new();
    private readonly DepositoryResolveScope _rootScope;

    public Depository(Action<DepositoryOption>? option = null)
    {
        option?.Invoke(_option);
        _rootScope = new DepositoryResolveScope(_option.ScopeOption);
        AddSelfToDepository();
    }
    
    private void AddSelfToDepository()
    {
        var description =
            new DependencyDescription(DependencyType: typeof(IDepository), Lifetime: DependencyLifetime.Singleton);
        var relation =
            new DependencyRelation(ImplementType: typeof(Depository), this);
        AddDependency(description);
        AddRelation(description, relation);
    }

    public void Dispose()
    {
        _dependencyDescriptions.Clear();
        _dependencyRelations.Clear();
        _rootScope.Dispose();
    }
}