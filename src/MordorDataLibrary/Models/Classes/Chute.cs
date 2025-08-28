namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Chute
{
    [NewRecord]
    public short X { get; set; }

    public short Y { get; set; }

    public short Depth { get; set; }
}
