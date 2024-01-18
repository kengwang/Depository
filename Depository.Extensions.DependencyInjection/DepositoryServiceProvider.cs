using System;
using System.Linq;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Extensions.DependencyInjection
{
    public class DepositoryServiceProvider : ISupportRequiredService, IKeyedServiceProvider, IServiceProviderIsKeyedService, IServiceScopeFactory
    {
        
        private readonly IDepository _depository;
        private readonly IDepositoryResolveScope? _resolveScope;

        public DepositoryServiceProvider(IDepository depository, IDepositoryResolveScope? resolveScope = null)
        {
            _depository = depository;
            _resolveScope = resolveScope;
        }
        
        public object GetService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption
            {
                ThrowWhenNotExists = false,
                Scope = _resolveScope
            });
        }

        public object GetRequiredService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                Scope = _resolveScope
            });
        }

        public object? GetKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                ThrowWhenNotExists = false,
                Scope = _resolveScope,
                RelationName = serviceKey?.GetHashCode().ToString()
            });
        }

        public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                RelationName = serviceKey?.GetHashCode().ToString(),
                Scope = _resolveScope
            });
        }

        public bool IsService(Type serviceType)
        {
            return _depository.DependencyExist(serviceType);
        }

        public bool IsKeyedService(Type serviceType, object? serviceKey)
        {
            var dependency = _depository.GetDependency(serviceType);
            return dependency is not null && _depository.GetRelations(dependency).Any(t=>t.Name == serviceKey?.GetHashCode().ToString());
        }

        public IServiceScope CreateScope()
        {
            return new DepositoryServiceScope(DepositoryResolveScope.Create(), _depository);
        }
    }
}