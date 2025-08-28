namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class HallRecord
{
    [NewRecord]
    public long Value { get; set; }

    [FixedLengthString(30)]
    public string Name { get; set; } = "";

    public int Date { get; set; }
}
