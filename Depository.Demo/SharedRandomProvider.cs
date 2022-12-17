namespace Depository.Demo;

public class SharedRandomProvider : IRandomProvider
{
    public int GetRandomNumber()
    {
        return Random.Shared.Next();
    }
}