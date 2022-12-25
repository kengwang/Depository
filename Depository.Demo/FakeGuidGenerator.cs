namespace Depository.Demo;

public class FakeGuidGenerator : IGuidGenerator
{
    public Guid GetGuid()
    {
        return Guid.Empty;
    }

    public int GetRandom()
    {
        return -1;
    }
}