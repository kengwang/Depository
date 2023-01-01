using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository : IDepository
{
    private readonly DepositoryOption _option = new();
    private readonly DepositoryResolveScope _rootScope = new();

    public Depository(Action<DepositoryOption>? option = null)
    {
        option?.Invoke(_option);
        AddSelfToDepository();
    }

    private async void AddSelfToDepository()
    {
        var description = new DependencyDescription
        {
            DependencyType = typeof(IDepository),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        var relation = new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(Depository)
        };
        await AddDependencyAsync(description);
        await AddRelationAsync(description, relation);
    }
    
    public Task RunAsync(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _dependencyDescriptions.Clear();
        _dependencyRelations.Clear();
        _usedImpls.Clear();
        _rootScope.Dispose();
    }
}