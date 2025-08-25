namespace MordorDataLibrary.Models;

[Serializable]
public class ItemType
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short IsEquippable { get; set; }
}
