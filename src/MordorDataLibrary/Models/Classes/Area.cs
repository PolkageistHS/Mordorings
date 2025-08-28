namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Area
{
    [NewRecord]
    public int SpawnMask { get; set; }

    /// <summary>
    /// If 0 or greater, the MonsterID for the laired monster
    /// </summary>
    public short LairId { get; set; }
}
