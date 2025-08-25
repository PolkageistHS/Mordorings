namespace MordorDataLibrary.Models;

[DataRecordLength(StaticValues.Data12RecordLength)]
[Serializable]
public class DATA12PartyGroups : IMordorDataFile
{
    [NewRecord]
    public short FakeCount { get; set; }

    public List<Party> Parties { get; set; } = [];
}
