namespace MordorDataLibrary.Models;

[Serializable]
public class Spell
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short ID { get; set; }

    public short Category { get; set; }

    public short Level { get; set; }

    public short Cost { get; set; }

    public short Unused { get; set; }

    public short KillEffect { get; set; }

    public short AffectMonster { get; set; }

    public short AffectGroup { get; set; }

    public short Damage1 { get; set; }

    public short Damage2 { get; set; }

    public short SpecialEffect { get; set; }

    public short[] RequiredStats { get; set; } = new short[7];

    public short ResistedBy { get; set; }

    public override string ToString() => Name;
}
