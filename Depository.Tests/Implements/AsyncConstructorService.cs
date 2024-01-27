using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class AsyncConstructorService : IAsyncConstructService, ICheckIsNormal
{
    public async Task InitializeService()
    {
        IsNormal = true;
    }

    public bool IsNormal { get; set; }
}