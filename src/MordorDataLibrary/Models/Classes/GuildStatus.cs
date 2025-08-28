namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class GuildStatus
{
    public short CurrentLevel { get; set; }

    public long CurrentExp { get; set; }

    public short IsQuested { get; set; }

    public short QuestedTargetId { get; set; }

    public short IsQuestCompleted { get; set; }

    public float Atk { get; set; }

    public float Def { get; set; }
}
