namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Guildmaster
{
    [NewRecord]
    public string Name { get; set; } = "";

    public short Level { get; set; }
}
