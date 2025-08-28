namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemType
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short IsEquippable { get; set; }
}
