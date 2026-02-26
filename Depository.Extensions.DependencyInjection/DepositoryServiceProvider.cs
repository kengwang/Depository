using System;
using System.Diagnostics.CodeAnalysis;
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

        public DepositoryServiceProvider(IDepository depository)
        {
            _depository = depository;
        }

        [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
        public object GetService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption
            {
                ThrowWhenNotExists = false
            });
        }

        [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
        public object GetRequiredService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType);
        }

        [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
        public object? GetKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                ThrowWhenNotExists = false,
                RelationName = Core.Depository.SafeToString(serviceKey)
            });
        }

        [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
        public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                RelationName = Core.Depository.SafeToString(serviceKey)
            });
        }

        public bool IsService(Type serviceType)
        {
            return _depository.DependencyExist(serviceType);
        }

        public bool IsKeyedService(Type serviceType, object? serviceKey)
        {
            var dependency = _depository.GetDependency(serviceType);
            return dependency is not null && _depository.GetRelations(dependency).Any(t=>t.Name == Core.Depository.SafeToString(serviceKey));
        }

        public IServiceScope CreateScope()
        {
            return new DepositoryServiceScope(DepositoryResolveScope.Create(), _depository);
        }
    }
}