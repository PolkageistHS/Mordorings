namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemSubtype
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short ItemType { get; set; }
}
