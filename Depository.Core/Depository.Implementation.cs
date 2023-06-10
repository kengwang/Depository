namespace Depository.Core;

public partial class Depository
{
    public async Task AddImplementation(Type implementType, object implement)
    {
        await _rootScope.AddImplementAsync(implementType, implement);
    }

    public async Task RemoveImplementation(Type implementType, object implement)
    {
        await _rootScope.RemoveImplementAsync(implementType, implement);
    }

    public async Task RemoveAllImplementation(Type implementType)
    {
        await _rootScope.RemoveAllImplementsAsync(implementType);
    }

    public async Task SetImplementation(Type implementType, object implement)
    {
        await _rootScope.SetImplementationAsync(implementType, implement);
    }
}