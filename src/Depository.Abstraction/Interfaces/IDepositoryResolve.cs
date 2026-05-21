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
    public List<object> ResolveDependencies([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency, DependencyResolveOption? option = null);

    /// <summary>
    /// Resolve Dependency in Depository
    /// </summary>
    /// <param name="dependency">Dependency Type</param>
    /// <param name="option"></param>
    /// <returns></returns>
    public object ResolveDependency([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency, DependencyResolveOption? option = null);

    public void ChangeResolveTarget([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency, object? target);
}
