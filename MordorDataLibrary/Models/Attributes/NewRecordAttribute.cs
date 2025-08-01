namespace MordorDataLibrary.Models;

/// <summary>
/// Indicates that the field is the start of a new record, so move the buffer forward
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class NewRecordAttribute : Attribute;
