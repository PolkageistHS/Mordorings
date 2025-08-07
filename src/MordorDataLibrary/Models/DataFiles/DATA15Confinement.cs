namespace MordorDataLibrary.Models;

[DataRecordLength(StaticValues.Data15RecordLength)]
public class DATA15Confinement : IMordorDataFile
{
    [NewRecord]
    public string Version { get; set; } = "";

    [NewRecord]
    public short Unused { get; set; }

    [NewRecord]
    public short TotalRecords { get; set; }

    public ConfinementMonster[] Monsters { get; set; } = new ConfinementMonster[StaticValues.MonsterCount];
}
