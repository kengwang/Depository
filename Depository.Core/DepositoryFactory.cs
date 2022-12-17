namespace Depository.Core;

public static class DepositoryFactory
{
    public static Depository CreateNew()
    {
        return new Depository();
    }
}