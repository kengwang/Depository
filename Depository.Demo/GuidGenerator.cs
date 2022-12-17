namespace Depository.Demo;

public class GuidGenerator : IGuidGenerator
{

    private readonly Guid _guid = Guid.NewGuid();

    public Guid GetGuid() => _guid;
}