#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis;

[Flags]
public enum DynamicallyAccessedMemberTypes
{
    None = 0,
    PublicParameterlessConstructor = 0x0001,
    PublicConstructors = 0x0003,
    NonPublicConstructors = 0x0004,
    PublicMethods = 0x0008,
    NonPublicMethods = 0x0010,
    PublicFields = 0x0020,
    NonPublicFields = 0x0040,
    PublicNestedTypes = 0x0080,
    NonPublicNestedTypes = 0x0100,
    PublicProperties = 0x0200,
    NonPublicProperties = 0x0400,
    PublicEvents = 0x0800,
    NonPublicEvents = 0x1000,
    Interfaces = 0x2000,
    All = ~None
}

[AttributeUsage(
    AttributeTargets.Field |
    AttributeTargets.ReturnValue |
    AttributeTargets.GenericParameter |
    AttributeTargets.Parameter |
    AttributeTargets.Property |
    AttributeTargets.Method |
    AttributeTargets.Class |
    AttributeTargets.Struct)]
public sealed class DynamicallyAccessedMembersAttribute : Attribute
{
    public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
    {
        MemberTypes = memberTypes;
    }

    public DynamicallyAccessedMemberTypes MemberTypes { get; }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
public sealed class RequiresUnreferencedCodeAttribute : Attribute
{
    public RequiresUnreferencedCodeAttribute(string message)
    {
        Message = message;
    }

    public string Message { get; }
    public string? Url { get; set; }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
public sealed class RequiresDynamicCodeAttribute : Attribute
{
    public RequiresDynamicCodeAttribute(string message)
    {
        Message = message;
    }

    public string Message { get; }
    public string? Url { get; set; }
}
#endif
