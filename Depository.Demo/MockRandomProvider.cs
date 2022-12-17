namespace Depository.Demo;

public class MockRandomProvider : IRandomProvider
{
    public int GetRandomNumber()
    {
        return 114514;
    }
}