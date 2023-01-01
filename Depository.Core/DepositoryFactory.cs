using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public static class DepositoryFactory
{
    public static Depository CreateNew(Action<DepositoryOption>? options = null)
    {
        return new Depository(options);
    }
}