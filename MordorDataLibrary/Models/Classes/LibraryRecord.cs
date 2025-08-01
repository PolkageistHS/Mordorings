namespace MordorDataLibrary.Models;

public class LibraryRecord
{
    [NewRecord]
    public short ID { get; set; }

    public short Known { get; set; }

    public long NumSeen { get; set; }

    [FixedLengthString(Length = 30)]
    public string LastSeenBy { get; set; } = "";

    public short MonsterID { get; set; } //ID of monster that dropped item, or 0 for monsters

    public int LastSeenDay { get; set; }

    [FixedLengthString(Length = 8)]
    public string Location { get; set; } = "";

    public short Deaths { get; set; } //0xFFFF for items
}
