namespace MordorDataLibrary.Models;

[DataRecordLength(StaticValues.Data14RecordLength)]
[Serializable]
public class DATA14HallOfRecords : IMordorDataFile
{
    public HallRecord[] Records { get; set; } = new HallRecord[13];
}
