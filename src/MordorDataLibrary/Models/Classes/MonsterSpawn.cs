namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MonsterSpawn
{
    public short Atk { get; set; }

    public short Def { get; set; }

    public short CurrentHp { get; set; }

    public short MaxHp { get; set; }

    public short Alignment { get; set; }

    public short Hostility { get; set; }

    public short MonsterId { get; set; }

    public short GroupSize { get; set; }

    public float SpawnTime { get; set; }

    public short IdentificationLevel { get; set; }

    public short NumberWhoWantToJoin { get; set; }

    public short OtherMonsterId { get; set; }
}
