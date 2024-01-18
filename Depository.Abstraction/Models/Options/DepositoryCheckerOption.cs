namespace Depository.Abstraction.Models.Options;

public class DepositoryCheckerOption
{
    public bool ImplementIsInheritedFromDependency { get; set; } = false;
    public bool ImplementIsInstantiable { get; set; } = false;
    public bool AutoConstructor { get; set; } = true;
    public bool CheckImplementationDuplication { get; set; } = true;
}