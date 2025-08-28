namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TreasureSpawn
{
    public short ChestType { get; set; }

    public short MonsterId { get; set; }

    public double Gold { get; set; }

    public short TrapId { get; set; }

    public short Locked { get; set; }
}
