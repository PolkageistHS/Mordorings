namespace MordorDataLibrary.Models;

[Serializable]
public class Guild
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short AverageHits { get; set; }

    public short MaxLevel { get; set; }

    public short MH { get; set; }

    public float ExpFactor { get; set; }

    public short Unused1 { get; set; }

    public short[] ReqStats { get; set; } = new short[7];

    public int Alignment { get; set; }

    public float[] AbilityRates { get; set; } = new float[7];

    public short Quested { get; set; }

    public float Unused2 { get; set; }

    public short QuestPercentage { get; set; }

    public float[] SpellGuildMods { get; set; } = new float[19];

    public float[] SpellCaps { get; set; } = new float[19];

    public int RaceMask { get; set; }

    public short GoldFactor { get; set; }

    public float LevelScale { get; set; }

    public float Atk { get; set; }

    public float Def { get; set; }

    public short MaxAtkDefIncreaseLevel { get; set; }

    public float AtkDefIncreaseRate { get; set; }

    public short Unused3 { get; set; }

    public short Unused4 { get; set; }

    public override string ToString() => Name;
}
