namespace Depository.Demo;

public class ActualGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.NewGuid();

    public int GetRandom() => Random.Shared.Next();
}