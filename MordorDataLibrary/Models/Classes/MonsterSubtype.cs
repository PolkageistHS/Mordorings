namespace MordorDataLibrary.Models;

public class MonsterSubtype
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short MonsterType { get; set; }
}
