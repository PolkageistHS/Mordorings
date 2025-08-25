namespace MordorDataLibrary.Models;

[Serializable]
public class DungeonMapTile
{
    [NewRecord]
    public short Area { get; set; }

    public long Tile { get; set; }
}
