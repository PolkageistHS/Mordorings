namespace MordorDataLibrary.Models;

[Serializable]
public class Chute
{
    [NewRecord]
    public short x { get; set; }

    public short y { get; set; }

    public short Depth { get; set; }
}
