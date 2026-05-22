using System;
using System.Linq;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Extensions.DependencyInjection
{
    public class DepositoryServiceProvider : ISupportRequiredService, IKeyedServiceProvider, IServiceProviderIsKeyedService, IServiceScopeFactory, IDisposable
    {
        
        private readonly IDepository _depository;
        private bool _disposed;

        public DepositoryServiceProvider(IDepository depository)
        {
            _depository = depository;
        }
        
        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption
            {
                ThrowWhenNotExists = false
            });
        }

        public object GetRequiredService(Type serviceType)
        {
            ThrowIfDisposed();
            return _depository.ResolveDependency(serviceType);
        }

        public object? GetKeyedService(Type serviceType, object? serviceKey)
        {
            ThrowIfDisposed();
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                ThrowWhenNotExists = false,
                RelationName = Core.Depository.SafeToString(serviceKey)
            });
        }

        public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            ThrowIfDisposed();
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                RelationName = Core.Depository.SafeToString(serviceKey)
            });
        }

        public bool IsService(Type serviceType)
        {
            ThrowIfDisposed();
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            return _depository.DependencyExist(serviceType);
        }

        public bool IsKeyedService(Type serviceType, object? serviceKey)
        {
            ThrowIfDisposed();
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                serviceType = serviceType.GenericTypeArguments[0];

            var dependency = _depository.GetDependency(serviceType);
            return dependency is not null && _depository.GetRelations(dependency).Any(t=>t.Name == Core.Depository.SafeToString(serviceKey));
        }

        public IServiceScope CreateScope()
        {
            ThrowIfDisposed();
            return new DepositoryServiceScope(DepositoryResolveScope.Create(), _depository);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _depository.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DepositoryServiceProvider));
        }
    }
}
