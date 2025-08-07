namespace MordorDataLibrary.Models;

[AttributeUsage(AttributeTargets.Property)]
public class FixedLengthStringAttribute : Attribute
{
    public ushort Length { get; init; }
}
