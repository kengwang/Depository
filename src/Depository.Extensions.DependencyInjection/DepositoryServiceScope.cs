using Depository.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Extensions.DependencyInjection;

public class DepositoryServiceScope : IServiceScope
{
    private readonly IDepositoryResolveScope _scope;
    private readonly IDepository _depository;
    private bool _disposed;

    public DepositoryServiceScope(IDepositoryResolveScope scope, IDepository depository)
    {
        _scope = scope;
        _depository = depository.CreateDepositoryInScope(_scope);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _scope.Dispose();
    }

    private IServiceProvider? _serviceProviderCache;

    public IServiceProvider ServiceProvider =>
        _disposed
            ? throw new ObjectDisposedException(nameof(DepositoryServiceScope))
            : _serviceProviderCache ??= new DepositoryServiceProvider(_depository);
}
