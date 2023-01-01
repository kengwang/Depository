namespace Depository.Abstraction.Models.Options;

public class DepositoryOption
{
    public bool AutoNotifyDependencyChange { get; set; } = false;
    public DependencyResolveOption ResolveOption { get; set; } = new();
    public DepositoryCheckerOption CheckerOption { get; set; } = new();
}