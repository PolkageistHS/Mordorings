namespace MordorDataLibrary.Models;

[Serializable]
public class LibraryRecord
{
    [NewRecord]
    public short ID { get; set; }

    public short Known { get; set; }

    public long NumSeen { get; set; }

    [FixedLengthString(30)]
    public string LastSeenBy { get; set; } = "";

    public short MonsterID { get; set; } //ID of monster that dropped item, or 0 for monsters

    public int LastSeenDay { get; set; }

    [FixedLengthString(8)]
    public string Location { get; set; } = "";

    public short Deaths { get; set; } //0xFFFF for items
}
