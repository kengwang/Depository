using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class GuidDecorationService : IDecorationService, IGuidGenerator
{

    private readonly IGuidGenerator _guidGenerator;

    public GuidDecorationService(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public Guid GetGuid()
    {
        if (_guidGenerator is not EmptyGuidGenerator) throw new Exception("Decoration cannot resolve original service");
        return _guidGenerator.GetGuid();
    }
}

public class MultipleGuidDecorationService : IDecorationService, IGuidGenerator
{

    private readonly List<IGuidGenerator> _guidGenerators;

    public MultipleGuidDecorationService(IEnumerable<IGuidGenerator> guidGenerators)
    {
        _guidGenerators = guidGenerators.ToList();
    }

    public Guid GetGuid()
    {
        if (_guidGenerators.Count != 2 && !_guidGenerators.Any(t=>t is MultipleGuidDecorationService)) throw new Exception("Decoration cannot resolve original service");
        return _guidGenerators[0].GetGuid();
    }
}