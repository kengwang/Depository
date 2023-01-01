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