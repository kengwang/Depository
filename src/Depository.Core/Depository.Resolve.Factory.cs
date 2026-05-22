using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Depository.Abstraction.Attributes;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Core;

public partial class Depository
{
    private readonly Dictionary<Type, ActivationMetadata> _activationMetadataCache = new();

    private object ResolveTypeToObject(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        var metadata = GetActivationMetadata(implementType, context);
        var parameters = ResolveParameters(implementType, metadata, context);
        return metadata.Constructor!.Invoke(parameters);
    }

    private ActivationMetadata GetActivationMetadata(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        if (_activationMetadataCache.TryGetValue(implementType, out var metadata)) return metadata;

        var constructorInfo = SelectConstructor(implementType, context);
        var parameters = constructorInfo.GetParameters();
        var parameterMetadata = new ParameterMetadata[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            parameterMetadata[i] = new ParameterMetadata(
                parameter.ParameterType,
                parameter.Name,
                parameter.Position,
                GetParameterServiceKey(parameter),
                parameter.HasDefaultValue,
                parameter.DefaultValue,
                parameter.IsOptional);
        }

        metadata = new ActivationMetadata(
            constructorInfo,
            parameterMetadata,
            typeof(IDecorationService).IsAssignableFrom(implementType));
        _activationMetadataCache[implementType] = metadata;
        return metadata;
    }

    private ConstructorInfo SelectConstructor(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        var constructorInfos = implementType.GetConstructors();
        if (constructorInfos.Length == 0)
            throw new DependencyInitializationException($"Cannot initialize {implementType.Name} with no constructor");

        var constructorInfo = constructorInfos[0];
        if (constructorInfos.Length == 1) return constructorInfo;

        foreach (var candidate in constructorInfos)
        {
            if (candidate.GetCustomAttributes(typeof(DepositoryActivatorConstructorAttribute), inherit: false).Length > 0)
                return candidate;
        }

        if (!Option.CheckerOption.AutoConstructor)
        {
            throw new DependencyInitializationException(
                $"More than one constructor was founded in {implementType.Name}, use DepositoryActivatorConstructorAttribute to define a DI constructor");
        }

        var max = 0;
        foreach (var info in constructorInfos)
        {
            var count = 0;
            foreach (var parameter in info.GetParameters())
            {
                if (parameter.IsOptional || parameter.HasDefaultValue ||
                    DependencyExist(parameter.ParameterType) ||
                    context.FixedImplementations?.ContainsKey(parameter.ParameterType) is true)
                {
                    count++;
                }
            }

            if (count <= max) continue;
            max = count;
            constructorInfo = info;
        }

        return constructorInfo;
    }

    public List<object> ResolveParameterInfos(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ParameterInfo[] parameterInfos,
        DependencyResolveOption? option)
    {
        var parameterMetadata = new ParameterMetadata[parameterInfos.Length];
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            var parameter = parameterInfos[i];
            parameterMetadata[i] = new ParameterMetadata(
                parameter.ParameterType,
                parameter.Name,
                parameter.Position,
                GetParameterServiceKey(parameter),
                parameter.HasDefaultValue,
                parameter.DefaultValue,
                parameter.IsOptional);
        }

        var metadata = new ActivationMetadata(null, parameterMetadata,
            typeof(IDecorationService).IsAssignableFrom(implementType));
        return ResolveParameters(implementType, metadata, ResolveContext.From(option))
            .Select(parameter => parameter!)
            .ToList();
    }

    private object?[] ResolveParameters(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ActivationMetadata metadata,
        ResolveContext context)
    {
        var parameters = new object?[metadata.Parameters.Length];
        for (var i = 0; i < metadata.Parameters.Length; i++)
        {
            var parameter = metadata.Parameters[i];
            object? resolveResult = null;
            if (context.FixedImplementations?.TryGetValue(parameter.ParameterType, out var impl) is true)
            {
                if (parameter.RelationName is not null && impl.TryGetValue(parameter.RelationName, out var value))
                {
                    parameters[i] = value;
                    continue;
                }

                if (impl.TryGetValue(string.Empty, out var defaultValue))
                {
                    resolveResult = defaultValue;
                }
            }
            else
            {
                var parameterContext = context
                    .WithRelationName(parameter.RelationName)
                    .WithThrowWhenNotExists(false)
                    .WithSkipDecoration(metadata.SkipDecorationForParameters);

                resolveResult = ResolveDependency(parameter.ParameterType, parameterContext);

                if (resolveResult is null && parameter.HasDefaultValue)
                {
                    resolveResult = parameter.DefaultValue;
                }
                else if (resolveResult is null && parameter.IsOptional)
                {
                    resolveResult = null;
                }
                else if (resolveResult is null)
                {
                    throw new DependencyInitializationException(
                        $"The constructor of {implementType.Name} contains a parameter called {parameter.Name} ({parameter.Position}) which cannot resolved");
                }
            }

            parameters[i] = resolveResult;
        }

        return parameters;
    }

    private string? GetParameterServiceKey(ParameterInfo parameterInfo)
    {
        if (Option.MicrosoftDependencyInjectionCompatible)
        {
            var fromKeyedServicesAttribute = parameterInfo.GetCustomAttribute<FromKeyedServicesAttribute>();
            if (fromKeyedServicesAttribute is not null)
            {
                return SafeToString(fromKeyedServicesAttribute.Key);
            }
        }

        return parameterInfo.GetCustomAttribute<FromNamedServiceAttribute>()?.Name;
    }

    private object ResolveRelation(
        DependencyDescription dependencyDescription,
        DependencyRelation relation,
        Type requestedDependency,
        ResolveContext context)
    {
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        if (relation.ImplementationFactory is not null) return relation.ImplementationFactory(this);

        var implementType = CloseGenericImplementation(requestedDependency, relation.ImplementType);
        return ResolveDescriptionWithImplementType(dependencyDescription, relation, requestedDependency, implementType, context);
    }

    private sealed class ActivationMetadata
    {
        public ActivationMetadata(ConstructorInfo? constructor, ParameterMetadata[] parameters, bool skipDecorationForParameters)
        {
            Constructor = constructor;
            Parameters = parameters;
            SkipDecorationForParameters = skipDecorationForParameters;
        }

        public ConstructorInfo? Constructor { get; }
        public ParameterMetadata[] Parameters { get; }
        public bool SkipDecorationForParameters { get; }
    }

    private readonly struct ParameterMetadata
    {
        public ParameterMetadata(
            Type parameterType,
            string? name,
            int position,
            string? relationName,
            bool hasDefaultValue,
            object? defaultValue,
            bool isOptional)
        {
            ParameterType = parameterType;
            Name = name;
            Position = position;
            RelationName = relationName;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
            IsOptional = isOptional;
        }

        public Type ParameterType { get; }
        public string? Name { get; }
        public int Position { get; }
        public string? RelationName { get; }
        public bool HasDefaultValue { get; }
        public object? DefaultValue { get; }
        public bool IsOptional { get; }
    }
}
