namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AreaSpawn
{
    [NewRecord]
    public MonsterSpawn MonsterSpawnGroup1 { get; set; } = null!;

    [NewRecord]
    public MonsterSpawn MonsterSpawnGroup2 { get; set; } = null!;

    [NewRecord]
    public MonsterSpawn MonsterSpawnGroup3 { get; set; } = null!;

    [NewRecord]
    public MonsterSpawn MonsterSpawnGroup4 { get; set; } = null!;

    [NewRecord]
    public TreasureSpawn Treasure { get; set; } = null!;
}
