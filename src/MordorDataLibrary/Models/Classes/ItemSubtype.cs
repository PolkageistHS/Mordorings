namespace MordorDataLibrary.Models;

[Serializable]
public class ItemSubtype
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short ItemType { get; set; }
}
