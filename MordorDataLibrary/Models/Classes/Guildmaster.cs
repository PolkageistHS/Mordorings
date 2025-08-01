namespace MordorDataLibrary.Models;

public class Guildmaster
{
    [NewRecord]
    public string Name { get; set; } = "";

    public short Level { get; set; }
}
