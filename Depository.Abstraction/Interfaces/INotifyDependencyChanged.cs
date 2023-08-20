namespace Depository.Abstraction.Interfaces;

public interface INotifyDependencyChanged<in T>
{
    /// <summary>
    /// Notify when dependency changed
    /// </summary>
    /// <param name="alwaysNullMarker">This parameter is ALWAYS NULL, DON'T USE IT</param>
    public void OnDependencyChanged(T? alwaysNullMarker);
}