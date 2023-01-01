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
    public Task<List<object>> ResolveDependenciesAsync(Type dependency, DependencyResolveOption? option = null);

    /// <summary>
    /// Resolve Dependency in Depository
    /// </summary>
    /// <param name="dependency">Dependency Type</param>
    /// <param name="option"></param>
    /// <returns></returns>
    public Task<object> ResolveDependencyAsync(Type dependency, DependencyResolveOption? option = null);

    public Task ChangeResolveTargetAsync(Type dependency, object? target);
}