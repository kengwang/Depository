using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Attributes;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    private void NotifyDependencyChange(DependencyDescription dependencyDescription, int mode = 0)
    {
        if (mode is 0 or 1)
        {
            // Notify List
            PostTypeChangeNotification(
                typeof(IEnumerable<>).MakeGenericType(dependencyDescription.DependencyType));
        }

        if (mode is 0 or 2)
        {
            // Notify Single
            PostTypeChangeNotification(dependencyDescription.DependencyType);
        }
    }

    private void PostTypeChangeNotification(Type type)
    {
        var notificationType = typeof(INotifyDependencyChanged<>).MakeGenericType(type);
        var description = GetDependencyDescription(notificationType);
        if (description is null) return;
        var relations = GetRelations(description);
        foreach (var relation in relations)
        {
            var result = ResolveRelation(description, relation);
            notificationType.GetMethods()[0].Invoke(result, new object?[] { null });
        }
    }

    public List<object> ResolveDependencies(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType && _dependencyDescriptions.All(t => t.DependencyType != dependency))
        {
            return ResolveGenericDependencies(dependency, option);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null)
            return option?.ThrowWhenNotExists is false ? new() : throw new DependencyNotFoundException(dependency);
        var relations = GetRelations(dependencyDescription, option?.IncludeDisabled is true);
        List<object> results = new();
        if (dependencyDescription.DecorationRelation is not null)
        {
            if (option?.SkipDecoration is not true)
            {
                results.Add(ResolveRelation(dependencyDescription, dependencyDescription.DecorationRelation, option));
                return results;
            }
        }

        foreach (var relation in relations)
        {
            if (relation.IsDecorationRelation) continue;
            var result = ResolveRelation(dependencyDescription, relation, option);
            results.Add(result);
        }

        return results;
    }

    public object ResolveDependency(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType)
        {
            if (dependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                // check whether is IEnumerable
                // and then return the fully Implemented stuff
                var cachedGenericType = dependency.GenericTypeArguments[0];
                var impls = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(cachedGenericType));
                if (!DependencyExist(cachedGenericType)) return impls;
                var resolves = ResolveDependencies(cachedGenericType, option);
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var impl in resolves)
                {
                    if (impl is null) continue;
                    if (cachedGenericType.IsInstanceOfType(impl))
                        impls.Add(impl);
                }

                return impls;
            }
            // ReSharper disable once RedundantIfElseBlock
            else if (dependency.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var actualType = dependency.GenericTypeArguments[0];
                if (DependencyExist(actualType)) return ResolveDependency(actualType, option);
                if (option?.ThrowWhenNotExists is true)
                    throw new DependencyNotFoundException(actualType);
                return null!;
            }
            else if (dependency.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var actualType = dependency.GenericTypeArguments[0];
                if (!DependencyExist(actualType))
                {
                    if (option?.ThrowWhenNotExists is true)
                        throw new DependencyNotFoundException(actualType);
                    return null!;
                }

                var previousCheckAsyncConstructor = option?.CheckAsyncConstructor ?? true;
                var newopt = option ?? new DependencyResolveOption();
                newopt.CheckAsyncConstructor = false;
                var result = ResolveDependency(actualType, newopt);
                if (result is not IAsyncConstructService asyncConstructService)
                {
                    return typeof(Task).GetMethod("FromResult")?.MakeGenericMethod(actualType).Invoke(null, new[] { result })!;
                }
                if (option is not null)
                    option.CheckAsyncConstructor = previousCheckAsyncConstructor;

                return Task.Run(async () =>
                {
                    await asyncConstructService.InitializeService();
                    return asyncConstructService;
                });

            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                // normal open-generic type
                // check if is implemented as an existed generic
                if (_dependencyDescriptions.All(t => t.DependencyType != dependency))
                {
                    return ResolveGenericDependency(dependency, option);
                }
            }
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null)
            return option?.ThrowWhenNotExists is false ? null! : throw new DependencyNotFoundException(dependency);
        DependencyRelation? relation;
        if (option?.SkipDecoration is not true && dependencyDescription.DecorationRelation is not null)
        {
            relation = dependencyDescription.DecorationRelation;
        }
        else
        {
            relation = GetRelation(dependencyDescription, option?.IncludeDisabled is true, option?.RelationName);
        }

        return relation is null ? null! : ResolveRelation(dependencyDescription, relation, option);
    }

    public void ChangeResolveTarget(Type dependency, object? target)
    {
        var description = GetDependencyDescription(dependency);
        if (description is null) throw new DependencyNotFoundException(dependency);
        if (description.Lifetime == DependencyLifetime.Singleton)
        {
            RootScope.SetImplementation(dependency, target);
        }

        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(description);
    }

    private object ResolveGenericDependency(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null)
            return option?.ThrowWhenNotExists is false ? null! : throw new DependencyNotFoundException(dependency);
        DependencyRelation? relation;
        if (option?.SkipDecoration is not true && dependencyDescription.DecorationRelation is not null)
        {
            relation = dependencyDescription.DecorationRelation;
        }
        else
        {
            relation = GetRelation(dependencyDescription, option?.IncludeDisabled is true);
        }

        if (relation is null) return null!;
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        if (relation.ImplementationFactory is not null) return relation.ImplementationFactory(this);
        var implementType = relation.ImplementType;
        if (!dependency.ContainsGenericParameters)
            implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
        return ResolveDescriptionWithImplementType(dependencyDescription, relation, dependency, implementType, option);
    }

    private List<object> ResolveGenericDependencies(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null)
            return option?.ThrowWhenNotExists is false ? new() : throw new DependencyNotFoundException(dependency);
        var relations = GetRelations(dependencyDescription, option?.IncludeDisabled is true);
        List<object> results = new();
        if (relations.FirstOrDefault(t => t.IsDecorationRelation) is { } decorationRelation)
        {
            if (option?.SkipDecoration is not true)
            {
                results.Add(ResolveRelation(dependencyDescription, decorationRelation, option));
                return results;
            }
        }

        foreach (var relation in relations)
        {
            if (relation.IsDecorationRelation) continue;
            if (relation.DefaultImplementation is not null) results.Add(relation.DefaultImplementation);
            if (relation.ImplementationFactory is not null) results.Add(relation.ImplementationFactory(this));
            var implementType = relation.ImplementType;
            if (!dependency.ContainsGenericParameters)
                implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
            var impl = ResolveDescriptionWithImplementType(dependencyDescription, relation, dependency, implementType,
                option);
            results.Add(impl);
        }

        return results;
    }

    private object ResolveTypeToObject(Type implementType, DependencyResolveOption? option)
    {
        var constructorInfos = implementType.GetConstructors();
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (constructorInfos.Length == 0)
            throw new DependencyInitializationException($"Cannot initialize {implementType.Name} with no constructor");
        var constructorInfo = constructorInfos[0];
        if (constructorInfos.Length > 1)
        {
            if (constructorInfos.FirstOrDefault(
                    c => c.CustomAttributes.Any(
                        att => att.AttributeType == typeof(DepositoryActivatorConstructorAttribute))
                ) is { } activatorInfo)
            {
                constructorInfo = activatorInfo;
            }
            else
            {
                if (Option.CheckerOption.AutoConstructor)
                {
                    var max = 0;
                    foreach (var info in constructorInfos)
                    {
                        var constructorParamInfos = info.GetParameters();
                        var count = 0;
                        foreach (var parameter in constructorParamInfos)
                        {
                            if (parameter.IsOptional || parameter.HasDefaultValue ||
                                DependencyExist(parameter.ParameterType) ||
                                option?.FatherImplementations?.ContainsKey(parameter.ParameterType) is true)
                            {
                                count++;
                            }
                            else
                            {
                                if (parameter.HasDefaultValue) count++;
                            }
                        }

                        if (count <= max) continue;
                        max = count;
                        constructorInfo = info;
                    }
                }
                else
                {
                    throw new DependencyInitializationException(
                        $"More than one constructor was founded in {implementType.Name}, use DepositoryActivatorConstructorAttribute to define a DI constructor");
                }
            }
        }


        var parameterInfos = constructorInfo.GetParameters();
        var parameters = ResolveParameterInfos(implementType, parameterInfos, option);
        
        var dependencyImpl = constructorInfo.Invoke(parameters.ToArray());
        if (option?.FatherImplementations is { Count: > 0 })
        {
            foreach (var kvp in option.FatherImplementations)
            {
                _fatherToChildRelation.GetOrCreateValue(kvp.Value).Add(new WeakReference(dependencyImpl));
                _childToFatherRelation.GetOrCreateValue(dependencyImpl).Add(new WeakReference(kvp.Value));
            }
        }
        
        return dependencyImpl;
    }

    public List<object> ResolveParameterInfos(Type implementType, ParameterInfo[] parameterInfos, DependencyResolveOption? option)
    {
        var previousThrowWhenNotExists = true;
        var parameters = new List<object>();
        foreach (var parameterInfo in parameterInfos)
        {
            if (option?.FatherImplementations?.TryGetValue(parameterInfo.ParameterType, out var impl) is true)
            {
                parameters.Add(impl);
            }
            else
            {
                option ??= new DependencyResolveOption();
                previousThrowWhenNotExists = option.ThrowWhenNotExists;
                var previousRelationName = option.RelationName;
                option.ThrowWhenNotExists = false;
                option.SkipDecoration = typeof(IDecorationService).IsAssignableFrom(implementType);
                if (Option.MicrosoftDependencyInjectionCompatible)
                {
                    var msattr = parameterInfo.GetCustomAttributes().FirstOrDefault(t=>t.GetType().FullName == "Microsoft.Extensions.DependencyInjection.FromKeyedServicesAttribute");
                    if (msattr is not null)
                    {
                        var key = msattr.GetType().GetProperty("Key")?.GetValue(msattr);
                        option.RelationName = SafeToString(key);
                    }
                }
                if (parameterInfo.GetCustomAttributes().FirstOrDefault(t => t is FromNamedServiceAttribute) is
                    FromNamedServiceAttribute fnsa)
                    option.RelationName = fnsa.Name;
                var resolveResult = ResolveDependency(parameterInfo.ParameterType, option);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (resolveResult != null)
                {
                    parameters.Add(resolveResult);
                    continue;
                }

                if (parameterInfo.HasDefaultValue)
                {
                    parameters.Add(parameterInfo.DefaultValue);
                    continue;
                }

                if (parameterInfo.IsOptional)
                {
                    parameters.Add(null!);
                }
                
                option.RelationName = previousRelationName;
                option.ThrowWhenNotExists = previousThrowWhenNotExists;
                
                throw new DependencyInitializationException(
                    $"The constructor of {implementType.Name} contains a parameter called {parameterInfo.Name} ({parameterInfo.Position}) which cannot resolved");
            }
        }
        if (option is not null)
            option.ThrowWhenNotExists = previousThrowWhenNotExists;
        return parameters;
    }


    private object ResolveRelation(
        DependencyDescription dependencyDescription,
        DependencyRelation relation,
        DependencyResolveOption? option = null)
    {
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        if (relation.ImplementationFactory is not null) return relation.ImplementationFactory(this);
        return ResolveDescriptionWithImplementType(dependencyDescription, relation,
            dependencyDescription.DependencyType, relation.ImplementType, option);
    }

    private object ResolveDescriptionWithImplementType(DependencyDescription description, DependencyRelation relation,
        Type inputType, Type implementType, DependencyResolveOption? option)
    {
        var impl = description.Lifetime switch
        {
            DependencyLifetime.Singleton => ResolveSingleton(implementType, option),
            DependencyLifetime.Transient => ResolveTransient(implementType, option),
            DependencyLifetime.Scoped => ResolveScoped(implementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (option?.CheckAsyncConstructor is not false &&
            impl is IAsyncConstructService asyncConstructService)
        {
            asyncConstructService.InitializeService().ConfigureAwait(false);
        }

        return impl;
    }

    private object ResolveScoped(Type implementType, DependencyResolveOption? option)
    {
        var scope = option?.Scope ?? CurrentScope;
        if (scope is null) throw new ScopeNotSetException();
        if (scope.Exist(implementType))
        {
            var ret = scope.GetImplement(implementType);
            if (ret is not null) return ret;
        }

        var impl = ResolveTypeToObject(implementType, option);
        scope.SetImplementation(implementType, impl);
        return impl;
    }

    private object ResolveTransient(Type implementType, DependencyResolveOption? option)
    {
        var impl = ResolveTypeToObject(implementType, option);
        return impl;
    }

    private object ResolveSingleton(Type implementType, DependencyResolveOption? option)
    {
        if (RootScope.Exist(implementType, option?.RelationName))
        {
            var ret = RootScope.GetImplement(implementType, option?.RelationName);
            if (ret is not null) return ret;
        }

        var impl = ResolveTypeToObject(implementType, option);
        RootScope.SetImplementation(implementType, impl, option?.RelationName);
        return impl;
    }
    
    internal static string SafeToString(object? obj)
    {
        if (obj == null)
            return "null";

        var type = obj.GetType();
        var toStringMethod = type.GetMethod("ToString", Type.EmptyTypes);

        // 检查是否在当前类型中重写了 ToString（排除继承自 object 的）
        if (toStringMethod != null && toStringMethod.DeclaringType != typeof(object))
        {
            return obj.ToString();
        }
        else
        {
            return $"{type.FullName}@{obj.GetHashCode():X}";
        }
    }
}