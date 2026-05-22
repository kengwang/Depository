namespace Depository.Core;

public partial class Depository
{
    public void RemoveImplementation(
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        string? key = null)
    {
        RootScope.RemoveImplement(implementType, key);
    }


    public void SetImplementation(
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        object implement,
        string? key = null)
    {
        RootScope.SetImplementation(implementType, implement);
    }
}
