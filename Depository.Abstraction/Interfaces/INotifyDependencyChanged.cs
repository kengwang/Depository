namespace Depository.Abstraction.Interfaces;

public interface INotifyDependencyChanged<in T>
{
    // !Notice: marker will always be null!
    public Task OnDependencyChanged(T? marker);
}