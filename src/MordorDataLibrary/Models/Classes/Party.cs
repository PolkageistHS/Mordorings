namespace MordorDataLibrary.Models;

[Serializable]
public class Party
{
    [NewRecord]
    [FixedLengthString(35)]
    public string Slot1 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(35)]
    public string Slot2 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(35)]
    public string Slot3 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(35)]
    public string Slot4 { get; set; } = "";
}
