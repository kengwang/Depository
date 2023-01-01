using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorIEnumerableInjectService : IConstructorInjectService
{
    public bool IsNormal { get; set; }

    public ConstructorIEnumerableInjectService(IEnumerable<IGuidGenerator> generators)
    {
        var guidGenerators = generators.ToList();
        IsNormal = guidGenerators.Count > 1;
    }
}