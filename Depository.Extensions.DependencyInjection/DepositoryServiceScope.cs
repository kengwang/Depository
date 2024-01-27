using Depository.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Extensions.DependencyInjection;

public class DepositoryServiceScope : IServiceScope
{
    private readonly IDepositoryResolveScope _scope;
    private readonly IDepository _depository;

    public DepositoryServiceScope(IDepositoryResolveScope scope, IDepository depository)
    {
        _scope = scope;
        _depository = depository.CreateDepositoryInScope(_scope);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    private IServiceProvider? _serviceProviderCache;

    public IServiceProvider ServiceProvider =>
        _serviceProviderCache ??= new DepositoryServiceProvider(_depository);
}