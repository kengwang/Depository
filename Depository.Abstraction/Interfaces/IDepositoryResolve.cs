using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolve
{
    /// <summary>
    /// Resolve Dependency in Depository
    /// </summary>
    /// <param name="dependency">Dependency Type</param>
    /// <param name="option"></param>
    /// <returns></returns>
    [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
    public List<object> ResolveDependencies(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type dependency,
        DependencyResolveOption? option = null);

    /// <summary>
    /// Resolve Dependency in Depository
    /// </summary>
    /// <param name="dependency">Dependency Type</param>
    /// <param name="option"></param>
    /// <returns></returns>
    [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
    public object ResolveDependency(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type dependency,
        DependencyResolveOption? option = null);

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void ChangeResolveTarget(Type dependency, object? target);
}