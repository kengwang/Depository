using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.Pipeline;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

[assembly:InternalsVisibleTo("Depository.Extensions.DependencyInjection")]

namespace Depository.Core;

public partial class Depository : IDepository
{
    public readonly DepositoryOption Option = new();

    [RequiresDynamicCode("Initialising Depository registers internal services using AddRelation.")]
    public Depository(Action<DepositoryOption>? option = null)
    {
        option?.Invoke(Option);
        RootScope = new DepositoryResolveScope(Option.ScopeOption);
        AddSelfToDepository();
        AddNotificationHubToDepository();
    }

    [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
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

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    private void AddNotificationHubToDepository()
    {
        var description =
            new DependencyDescription(dependencyType: typeof(INotificationHub), lifetime: DependencyLifetime.Singleton);
        var relation =
            new DependencyRelation(ImplementType: typeof(NotificationHub));
        AddDependency(description);
        AddRelation(description, relation);
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
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
        RootScope.Dispose();
    }


}