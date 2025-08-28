namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ConfinementMonster
{
    [NewRecord]
    public short RowNumber { get; set; }

    public short MonsterId { get; set; }

    public short Good { get; set; }

    public short Neutral { get; set; }

    public short Evil { get; set; }
}
