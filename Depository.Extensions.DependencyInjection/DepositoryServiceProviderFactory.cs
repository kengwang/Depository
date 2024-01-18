using System;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Extensions.DependencyInjection
{
    public class DepositoryServiceProviderFactory : IServiceProviderFactory<IDepository>
    {
        private readonly Action<DepositoryOption>? _options = null;

        public DepositoryServiceProviderFactory(Action<DepositoryOption>? options = null)
        {
            _options = options;
        }

        public IDepository CreateBuilder(IServiceCollection services)
        {
            var depository = DepositoryFactory.CreateNew(_options);
            foreach (var serviceDescriptor in services)
            {
                if (serviceDescriptor.ImplementationType is null &&
                    serviceDescriptor.ImplementationInstance is null &&
                    serviceDescriptor.ImplementationFactory is null &&
                    serviceDescriptor.KeyedImplementationFactory is null)
                    continue;
                var dependency = depository.GetDependency(serviceDescriptor.ServiceType);
                if (dependency is null)
                {
                    dependency = new DependencyDescription(serviceDescriptor.ServiceType,
                        serviceDescriptor.Lifetime switch
                        {
                            ServiceLifetime.Singleton => DependencyLifetime.Singleton,
                            ServiceLifetime.Transient => DependencyLifetime.Transient,
                            ServiceLifetime.Scoped => DependencyLifetime.Scoped,
                            _ => throw new ArgumentOutOfRangeException()
                        });
                    depository.AddDependency(dependency);
                }

                var relation = new DependencyRelation(serviceDescriptor.ImplementationType!)
                {
                    DefaultImplementation = serviceDescriptor.ImplementationInstance,
                    Name = serviceDescriptor.ServiceKey?.GetHashCode().ToString(),
                };
                if (serviceDescriptor.ImplementationFactory is not null)
                {
                    relation.ImplementationFactory = (resolvingDepository) =>
                        serviceDescriptor.ImplementationFactory.Invoke(
                            new DepositoryServiceProvider(resolvingDepository));
                }

                if (serviceDescriptor is { IsKeyedService: true, KeyedImplementationFactory: not null })
                {
                    // TODO: Support Keyed Implementation Factory to pass down the key
                    relation.ImplementationFactory = (resolvingDepository) =>
                        serviceDescriptor.KeyedImplementationFactory.Invoke(
                            new DepositoryServiceProvider(resolvingDepository), null);
                }
                
                depository.AddRelation(dependency, relation);
            }

            return depository;
        }

        public IServiceProvider CreateServiceProvider(IDepository containerBuilder)
        {
            var sp = new DepositoryServiceProvider(containerBuilder);
            QuickAddSingleton<IServiceProvider, DepositoryServiceProvider>(containerBuilder, sp);
            QuickAddSingleton<IServiceProviderIsService, DepositoryServiceProvider>(containerBuilder, sp);
            QuickAddSingleton<IKeyedServiceProvider, DepositoryServiceProvider>(containerBuilder, sp);
            QuickAddSingleton<IServiceProviderIsKeyedService, DepositoryServiceProvider>(containerBuilder, sp);
            QuickAddSingleton<IServiceScopeFactory, DepositoryServiceProvider>(containerBuilder, sp);
            QuickAddSingleton<IServiceProviderFactory<IDepository>, DepositoryServiceProviderFactory>(containerBuilder,
                this);
            return sp;
        }

        private static void QuickAddSingleton<TService, TImplement>(IDepository depository, TService sp)
        {
            var dependency = new DependencyDescription(typeof(TService), DependencyLifetime.Singleton);
            depository.AddDependency(dependency);
            var relation = new DependencyRelation(typeof(TImplement), sp);
            depository.AddRelation(dependency, relation);
        }
    }
}