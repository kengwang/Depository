namespace Depository.Abstraction.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromNamedServiceAttribute : Attribute
{
    public string Name;
    public FromNamedServiceAttribute(string name)
    {
        Name = name;
    }
}