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
}