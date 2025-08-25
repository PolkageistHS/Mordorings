namespace MordorDataLibrary.Models;

[Serializable]
public class Race
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short[] MinStats { get; set; } = new short[7];

    public short[] MaxStats { get; set; } = new short[7];

    public short[] Resistances { get; set; } = new short[12];

    public int Alignment { get; set; }

    public short Size { get; set; }

    public short BonusPoints { get; set; }

    public short MaxAge { get; set; }

    public float ExpFactor { get; set; }
}
