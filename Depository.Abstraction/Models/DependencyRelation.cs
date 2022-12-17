using Depository.Abstraction.Enums;

namespace Depository.Abstraction.Models;

public class DependencyRelation
{
    public DependencyRelationType RelationType { get; set; }
    public Type ImplementType { get; set; }
    public object? DefaultImplementation { get; set; }
}