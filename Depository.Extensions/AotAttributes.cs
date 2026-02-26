// Polyfill AOT-related attributes for netstandard2.0 targets.
#if !NET7_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class,
        Inherited = false)]
    internal sealed class RequiresDynamicCodeAttribute : Attribute
    {
        public RequiresDynamicCodeAttribute(string message) => Message = message;
        public string Message { get; }
        public string? Url { get; set; }
    }
}
#endif
