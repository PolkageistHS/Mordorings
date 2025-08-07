namespace MordorDataLibrary.Models;

public class HallRecord
{
    [NewRecord]
    public long Value { get; set; }

    [FixedLengthString(Length = 30)]
    public string Name { get; set; } = "";

    public int Date { get; set; }
}
