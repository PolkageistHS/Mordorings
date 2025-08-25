namespace MordorDataLibrary.Models;

[Serializable]
public class Teleporter
{
    [NewRecord]
    public short x { get; set; }

    public short y { get; set; }

    public short x2 { get; set; }

    public short y2 { get; set; }

    public short z2 { get; set; }
}
