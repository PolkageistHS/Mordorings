namespace MordorDataLibrary.Models;

[Serializable]
public class GuildLogEntry
{
    public short GuildID { get; set; }

    public int DayValue { get; set; }

    public string Message { get; set; } = "";
}
