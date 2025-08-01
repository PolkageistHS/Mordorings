namespace MordorDataLibrary.Models;

[DataRecordLength(StaticValues.Data07RecordLength)]
public class DATA07Guildmasters : IMordorDataFile
{
    [NewRecord]
    public string Version { get; set; } = null!;

    public Guildmaster[] GuildmasterList { get; set; } = new Guildmaster[12];
}
