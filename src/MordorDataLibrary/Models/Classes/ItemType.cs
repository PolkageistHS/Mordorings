namespace MordorDataLibrary.Models;

public class ItemType
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short IsEquippable { get; set; }
}
