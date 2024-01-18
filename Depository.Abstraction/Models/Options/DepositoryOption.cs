namespace Depository.Abstraction.Models.Options;

public class DepositoryOption
{
    public bool AutoNotifyDependencyChange { get; set; } = false;
    public ImplementTypeDuplicatedAction ImplementTypeDuplicatedAction { get; set; } =
        ImplementTypeDuplicatedAction.Ignore;
    public DepositoryCheckerOption CheckerOption { get; set; } = new();
    public DepositoryResolveScopeOption ScopeOption { get; set; } = new();
}

public enum ImplementTypeDuplicatedAction
{
    Throw, // This will throw and exception to let you know
    Ignore, // This will add implement only one time
    Continue // This will add implement two times
}