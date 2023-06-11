using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class ConstructorIEnumerableInjectService : IConstructorInjectService, ICheckIsNormal
{
    public bool IsNormal { get; set; }

    public ConstructorIEnumerableInjectService(IEnumerable<IGuidGenerator> generators)
    {
        var guidGenerators = generators.ToList();
        IsNormal = guidGenerators.Count > 1;
    }
}