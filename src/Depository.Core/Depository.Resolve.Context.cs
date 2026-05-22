using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

internal readonly struct ResolveContext
{
    private ResolveContext(
        IDepositoryResolveScope? scope,
        bool includeDisabled,
        bool skipDecoration,
        string? relationName,
        bool checkAsyncConstructor,
        bool throwWhenNotExists,
        Dictionary<Type, Dictionary<string, object>>? fixedImplementations)
    {
        Scope = scope;
        IncludeDisabled = includeDisabled;
        SkipDecoration = skipDecoration;
        RelationName = relationName;
        CheckAsyncConstructor = checkAsyncConstructor;
        ThrowWhenNotExists = throwWhenNotExists;
        FixedImplementations = fixedImplementations;
    }

    public static ResolveContext Default { get; } = new(null, false, false, null, true, true, null);

    public IDepositoryResolveScope? Scope { get; }
    public bool IncludeDisabled { get; }
    public bool SkipDecoration { get; }
    public string? RelationName { get; }
    public bool CheckAsyncConstructor { get; }
    public bool ThrowWhenNotExists { get; }
    public Dictionary<Type, Dictionary<string, object>>? FixedImplementations { get; }

    public static ResolveContext From(DependencyResolveOption? option)
    {
        return option is null
            ? Default
            : new ResolveContext(
                option.Scope,
                option.IncludeDisabled,
                option.SkipDecoration,
                option.RelationName,
                option.CheckAsyncConstructor,
                option.ThrowWhenNotExists,
                option.FixedImplementations);
    }

    public ResolveContext WithScope(IDepositoryResolveScope? scope)
    {
        return new ResolveContext(scope, IncludeDisabled, SkipDecoration, RelationName, CheckAsyncConstructor,
            ThrowWhenNotExists, FixedImplementations);
    }

    public ResolveContext WithRelationName(string? relationName)
    {
        return new ResolveContext(Scope, IncludeDisabled, SkipDecoration, relationName, CheckAsyncConstructor,
            ThrowWhenNotExists, FixedImplementations);
    }

    public ResolveContext WithSkipDecoration(bool skipDecoration)
    {
        return new ResolveContext(Scope, IncludeDisabled, skipDecoration, RelationName, CheckAsyncConstructor,
            ThrowWhenNotExists, FixedImplementations);
    }

    public ResolveContext WithThrowWhenNotExists(bool throwWhenNotExists)
    {
        return new ResolveContext(Scope, IncludeDisabled, SkipDecoration, RelationName, CheckAsyncConstructor,
            throwWhenNotExists, FixedImplementations);
    }

    public ResolveContext WithCheckAsyncConstructor(bool checkAsyncConstructor)
    {
        return new ResolveContext(Scope, IncludeDisabled, SkipDecoration, RelationName, checkAsyncConstructor,
            ThrowWhenNotExists, FixedImplementations);
    }
}
