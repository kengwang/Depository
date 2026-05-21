using System.Diagnostics.CodeAnalysis;

namespace Depository.Abstraction.Interfaces;

public interface IDepositoryImplementation
{
    public void RemoveImplementation([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType, string? key = null);
    public void SetImplementation([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType, object implement, string? key = null);
}
