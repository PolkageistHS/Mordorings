namespace MordorDataLibrary.Models;

[Serializable]
public class MonsterType
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short Unused { get; set; }

    public override string ToString() => Name;
}
