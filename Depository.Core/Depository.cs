using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.Pipeline;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository : IDepository
{
    private readonly DepositoryOption _option = new();
    private readonly DepositoryResolveScope _rootScope;

    public Depository(Action<DepositoryOption>? option = null)
    {
        option?.Invoke(_option);
        _rootScope = new DepositoryResolveScope(_option.ScopeOption);
        AddSelfToDepository();
        AddNotificationHubToDepository();
    }

    private IPipeline<TContext, TReturnValue> GetOrCreatePipeline<TContext, TReturnValue>()
        where TContext : IPipelineContext<TContext, TReturnValue> where TReturnValue : class
    {
        if (!DependencyExist(typeof(IPipeline<TContext, TReturnValue>)))
        {
            var description =
                new DependencyDescription(dependencyType: typeof(IPipeline<TContext, TReturnValue>),
                                          lifetime: DependencyLifetime.Singleton);
            var relation =
                new DependencyRelation(ImplementType: typeof(PipelineHub<TContext, TReturnValue>));
            AddDependency(description);
            AddRelation(description, relation);
        }

        return (IPipeline<TContext, TReturnValue>)ResolveDependency(typeof(IPipeline<TContext, TReturnValue>));
    }

    private void AddNotificationHubToDepository()
    {
        var description =
            new DependencyDescription(dependencyType: typeof(INotificationHub), lifetime: DependencyLifetime.Singleton);
        var relation =
            new DependencyRelation(ImplementType: typeof(NotificationHub));
        AddDependency(description);
        AddRelation(description, relation);
    }

    private void AddSelfToDepository()
    {
        var description =
            new DependencyDescription(dependencyType: typeof(IDepository), lifetime: DependencyLifetime.Singleton);
        var relation =
            new DependencyRelation(ImplementType: typeof(Depository), this);
        AddDependency(description);
        AddRelation(description, relation);
    }

    public void Dispose()
    {
        _dependencyDescriptions.Clear();
        _dependencyRelations.Clear();
        _rootScope.Dispose();
    }
}