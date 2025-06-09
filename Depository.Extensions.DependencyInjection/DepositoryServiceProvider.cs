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

        public DepositoryServiceProvider(IDepository depository)
        {
            _depository = depository;
        }
        
        public object GetService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption
            {
                ThrowWhenNotExists = false
            });
        }

        public object GetRequiredService(Type serviceType)
        {
            return _depository.ResolveDependency(serviceType);
        }

        public object? GetKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                ThrowWhenNotExists = false,
                RelationName = DepositoryServiceProviderFactory.SafeToString(serviceKey)
            });
        }

        public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return _depository.ResolveDependency(serviceType, new DependencyResolveOption()
            {
                RelationName = DepositoryServiceProviderFactory.SafeToString(serviceKey)
            });
        }

        public bool IsService(Type serviceType)
        {
            return _depository.DependencyExist(serviceType);
        }

        public bool IsKeyedService(Type serviceType, object? serviceKey)
        {
            var dependency = _depository.GetDependency(serviceType);
            return dependency is not null && _depository.GetRelations(dependency).Any(t=>t.Name == DepositoryServiceProviderFactory.SafeToString(serviceKey));
        }

        public IServiceScope CreateScope()
        {
            return new DepositoryServiceScope(DepositoryResolveScope.Create(), _depository);
        }
    }
}