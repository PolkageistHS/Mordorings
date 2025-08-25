namespace MordorDataLibrary.Models;

[Serializable]
public class Companion
{
    [FixedLengthString(15)]
    public string Name { get; set; } = null!;

    public short MonsterID { get; set; }

    public short Slot { get; set; }

    public short CurrentHp { get; set; }

    public short MaxHp { get; set; }

    public short Alignment { get; set; }

    public short Atk { get; set; }

    public short Def { get; set; }

    public short BindLevel { get; set; }

    public short IDLevel { get; set; }
}
