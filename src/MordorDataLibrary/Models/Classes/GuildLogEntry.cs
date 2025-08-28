namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class GuildLogEntry
{
    public short GuildId { get; set; }

    public int DayValue { get; set; }

    public string Message { get; set; } = "";
}
