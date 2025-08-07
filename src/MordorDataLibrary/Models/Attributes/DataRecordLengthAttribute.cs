using System.Diagnostics;

namespace MordorDataLibrary.Models;

[AttributeUsage(AttributeTargets.Class)]
[method: DebuggerStepThrough]
public class DataRecordLengthAttribute(int length) : Attribute
{
    public int? Length { get; } = length;
}
