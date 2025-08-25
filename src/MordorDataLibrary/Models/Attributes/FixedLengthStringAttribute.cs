namespace MordorDataLibrary.Models;

[AttributeUsage(AttributeTargets.Property)]
public class FixedLengthStringAttribute(ushort length) : Attribute
{
    public ushort Length { get; } = length;
}
