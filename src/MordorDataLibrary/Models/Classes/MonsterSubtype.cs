namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MonsterSubtype
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short MonsterType { get; set; }

    public override string ToString() => Name;
}
