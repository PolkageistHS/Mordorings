namespace MordorDataLibrary.Models;

public class Party
{
    [NewRecord]
    [FixedLengthString(Length = 35)]
    public string Slot1 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(Length = 35)]
    public string Slot2 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(Length = 35)]
    public string Slot3 { get; set; } = "";

    [NewRecord]
    [FixedLengthString(Length = 35)]
    public string Slot4 { get; set; } = "";
}
