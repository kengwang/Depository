using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository : IDepository
{
    private readonly DepositoryOption _option = new();
    private readonly DepositoryResolveScope _rootScope = new();
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